using System.Data;
using System.Globalization;
using System.Security.Claims;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

public sealed class PaymentService(IDbContextFactory<CafeDbContext> factory, StaffAccess staff)
{
    public async Task<(StaffPaymentStatus? Value, OperationError? Error)> GetStaffAsync(ClaimsPrincipal principal,
        long id, CancellationToken ct = default)
    {
        try
        {
            var denied = await CheckCashierAsync(principal);
            if (denied is not null) return (null, denied);
            await using var db = await factory.CreateDbContextAsync(ct);
            var order = await db.Orders.AsNoTracking().SingleOrDefaultAsync(o => o.Id == id, ct);
            return order is null ? (null, Missing()) : (await StaffViewAsync(db, order, ct), null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception) { return (null, Uncertain()); }
    }

    public async Task<(PaymentStatus? Value, OperationError? Error)> GetGuestAsync(string? capability, CancellationToken ct = default)
    {
        try
        {
            await using var db = await factory.CreateDbContextAsync(ct);
            var id = await FindCustomerIdAsync(db, capability, ct);
            if (id is null) return (null, Missing());
            var order = await db.Orders.AsNoTracking().SingleAsync(o => o.Id == id, ct);
            return (await ViewAsync(db, order, ct), null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception) { return (null, Uncertain()); }
    }

    public async Task<(StaffPaymentStatus? Value, OperationError? Error)> RecordCashAsync(ClaimsPrincipal principal,
        long id, CashPaymentInput input, CancellationToken ct = default)
    {
        var version = input.RowVersion;
        try
        {
            var denied = await CheckCashierAsync(principal);
            if (denied is not null) return (null, denied);
            byte[] expected;
            try { expected = Convert.FromBase64String(version); }
            catch (Exception ex) when (ex is FormatException or ArgumentNullException)
            { return (null, new(OperationErrorKind.Invalid, "Reload the bill before recording cash.")); }
            if (expected.Length != 8) return (null, new(OperationErrorKind.Invalid, "Reload the bill before recording cash."));
            await using var db = await factory.CreateDbContextAsync(ct);
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var order = await PaymentRules.LockOrderAsync(db, id, ct);
            if (order is null) return (null, Missing());
            var existing = await db.PaymentSettlements.AsNoTracking().SingleOrDefaultAsync(p => p.OrderId == id, ct);
            if (existing is not null)
                return existing.Method == "Cash" ? (await StaffViewAsync(db, order, ct), null)
                    : (null, Conflict("This order is already paid online."));
            if (order.Status == OrderStatus.Rejected) return (null, Conflict("Rejected orders cannot receive a payment."));
            if (await db.PaymentAttempts.AnyAsync(p => p.OrderId == id && p.IsBlocking, ct))
                return (null, Conflict("Reconcile the pending or uncertain online payment before recording cash."));
            if (!order.RowVersion.SequenceEqual(expected)) return (null, Conflict("The order changed. Reload the bill first."));
            db.PaymentSettlements.Add(new()
            {
                OrderId = id, Amount = order.Total, Currency = order.Currency, Method = "Cash",
                RecordedByUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            });
            order.PaymentRevision = checked(order.PaymentRevision + 1);
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (await StaffViewAsync(db, order, ct), null);
        }
        catch (Exception) { return (null, Uncertain()); }
    }

    internal static async Task<long?> FindCustomerIdAsync(CafeDbContext db, string? capability, CancellationToken ct)
    {
        if (!OrderService.TryHashCapability(capability, out var hash)) return null;
        return await db.Orders.AsNoTracking().Where(o => o.Origin == OrderOrigin.Customer && o.CapabilityHash == hash)
            .Select(o => (long?)o.Id).SingleOrDefaultAsync(ct);
    }

    private async Task<OperationError?> CheckCashierAsync(ClaimsPrincipal principal)
    {
        var access = await staff.CheckCashierAsync(principal);
        return access == StaffAccessResult.Allowed ? null : new(access == StaffAccessResult.Unauthenticated
            ? OperationErrorKind.Unauthenticated : OperationErrorKind.Forbidden, "Current cashier access is required.");
    }

    private async Task<StaffPaymentStatus> StaffViewAsync(CafeDbContext db, Order order, CancellationToken ct)
    {
        var actor = await db.PaymentSettlements.AsNoTracking().Where(p => p.OrderId == order.Id)
            .Select(p => p.RecordedByUserId).SingleOrDefaultAsync(ct);
        return new(await ViewAsync(db, order, ct), Convert.ToBase64String(order.RowVersion), actor);
    }

    private async Task<PaymentStatus> ViewAsync(CafeDbContext db, Order order, CancellationToken ct)
    {
        var payment = await db.PaymentSettlements.AsNoTracking().SingleOrDefaultAsync(p => p.OrderId == order.Id, ct);
        var active = payment is null ? await db.PaymentAttempts.AsNoTracking()
            .SingleOrDefaultAsync(p => p.OrderId == order.Id && p.IsBlocking, ct) : null;
        var state = payment is not null ? payment.Method == "Cash" ? "PaidCash" : "PaidSandbox"
            : active is null ? "Unpaid" : active.State is "Uncertain" or "Creating" ? "Uncertain" : "Pending";
        return new(state, payment?.Method, order.Total.ToString("F2", CultureInfo.InvariantCulture), order.Currency,
            payment?.RecordedAtUtc, false);
    }

    private static OperationError Missing() => new(OperationErrorKind.NotFound, "This order payment could not be found.");
    private static OperationError Conflict(string message) => new(OperationErrorKind.Conflict, message, "payment_conflict");
    private static OperationError Uncertain() => new(OperationErrorKind.Unexpected,
        "Unable to confirm the payment outcome. Reload the bill before any further action.", "payment_outcome_uncertain");
}
