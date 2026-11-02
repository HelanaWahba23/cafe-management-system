using System.Globalization;
using System.Data;
using System.Security.Claims;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

public sealed class PosOrderService(IDbContextFactory<CafeDbContext> factory, StaffAccess staff, OrderService orders)
{
    public Task<(PosTable[]? Value, OperationError? Error)> TablesAsync(ClaimsPrincipal principal,
        CancellationToken ct = default) => ExecuteAsync<PosTable[]>(principal, async db =>
        (await db.CafeTables.AsNoTracking().OrderBy(t => t.Label).ThenBy(t => t.Id)
            .Select(t => new PosTable(t.PublicCode, t.Label)).ToArrayAsync(ct), null), ct);

    public Task<(PickupMenuDetails? Value, OperationError? Error)> CatalogAsync(ClaimsPrincipal principal,
        CancellationToken ct = default) => ExecuteAsync<PickupMenuDetails>(principal, async db =>
        (new("EGP", await db.MenuCategories.AsNoTracking().OrderBy(c => c.Id)
            .Select(c => new PublicMenuCategory(c.Id, c.Name, c.Products.Where(p => p.IsAvailable).OrderBy(p => p.Id)
                .Select(p => new PublicMenuProduct(p.Id, p.Name, p.Price)).ToArray())).ToArrayAsync(ct)), null), ct);

    public Task<(OrderReview? Value, OperationError? Error)> ReviewAsync(ClaimsPrincipal principal,
        CounterOrderInput input, CancellationToken ct = default)
    {
        var attempt = input.AttemptKey; var snapshot = input.Snapshot();
        return ExecuteAsync<OrderReview>(principal, _ => orders.ReviewCounterAsync(attempt,
            snapshot.Draft, principal.FindFirstValue(ClaimTypes.NameIdentifier)!, ct), ct);
    }

    public async Task<CounterOutcome> PlaceAsync(ClaimsPrincipal principal, CounterOrderInput input, CancellationToken ct = default)
    {
        var attempt = input.AttemptKey; var snapshot = input.Snapshot();
        var result = await ExecuteAsync<CounterOutcome>(principal, async db =>
        {
            var placed = await orders.PlaceCounterAsync(attempt, snapshot,
                principal.FindFirstValue(ClaimTypes.NameIdentifier)!, ct);
            if (placed.Error is not null) return (new(null, placed.Error, Review: placed.Review), null);
            var reference = placed.Value!.Order.Reference;
            var id = await db.Orders.Where(o => o.Reference == reference && o.Origin == OrderOrigin.Counter
                && o.CreatedByUserId == principal.FindFirstValue(ClaimTypes.NameIdentifier)).Select(o => o.Id).SingleAsync(ct);
            return (new(new(reference, id), null, placed.Created), null);
        }, ct);
        return result.Error is not null ? new(null, result.Error) : result.Value!;
    }

    public Task<(PosOrderPage? Value, OperationError? Error)> ListAsync(ClaimsPrincipal principal,
        string? status = null, int page = 1, CancellationToken ct = default) => ExecuteAsync<PosOrderPage>(principal, async db =>
    {
        if (page is < 1 or > 1000000 || status is not null && !ValidStatus(status, out _))
            return (null, Invalid("Choose a valid status and page."));
        var query = db.Orders.AsNoTracking();
        if (status is not null) { var selected = Enum.Parse<OrderStatus>(status); query = query.Where(o => o.Status == selected); }
        var count = await query.CountAsync(ct);
        var rows = await query.OrderByDescending(o => o.CreatedAtUtc).ThenByDescending(o => o.Id)
            .Skip((page - 1) * 50).Take(50).ToArrayAsync(ct);
        return (new(rows.Select(o => new PosOrderSummary(o.Id, o.Reference, o.Origin.ToString(),
            o.Type.ToString(), o.TableLabel, o.Status.ToString(), Money(o.Total), o.CreatedAtUtc)).ToArray(), count, page, 50), null);
    }, ct);

    public Task<(PosOrderDetail? Value, OperationError? Error)> GetAsync(ClaimsPrincipal principal,
        long id, CancellationToken ct = default) => ExecuteAsync<PosOrderDetail>(principal, async db =>
    {
        if (id <= 0) return (null, Invalid("Choose a valid order."));
        var order = await db.Orders.AsNoTracking().Include(o => o.Lines).SingleOrDefaultAsync(o => o.Id == id, ct);
        return order is null ? (null, Missing()) : (Project(order), null);
    }, ct);

    public Task<(PosOrderDetail? Value, OperationError? Error)> ChangeStatusAsync(ClaimsPrincipal principal,
        long id, PosStatusInput input, CancellationToken ct = default)
    {
        // Copy mutable input before any await.
        var status = input.Status; var version = input.RowVersion; var reason = input.RejectionReason;
        return ExecuteAsync<PosOrderDetail>(principal, async db =>
        {
            if (id <= 0 || !ValidStatus(status, out var next)) return (null, Invalid("Choose a valid order status."));
            byte[] expected;
            try { expected = Convert.FromBase64String(version); }
            catch (Exception ex) when (ex is FormatException or ArgumentNullException) { return (null, Invalid("Reload the order before changing status.")); }
            if (expected.Length != 8) return (null, Invalid("Reload the order before changing status."));
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var order = await PaymentRules.LockOrderAsync(db, id, ct);
            if (order is null) return (null, Missing());
            if (!order.RowVersion.SequenceEqual(expected)) return (null, Stale());
            if (next == OrderStatus.Rejected && await PaymentRules.BlocksRejectionAsync(db, id, ct))
                return (null, new(OperationErrorKind.Conflict, "Paid orders cannot be rejected. Reconcile any pending or uncertain payment first.", "payment_conflict"));
            if (!OrderLifecycle.TryTransition(order, next, reason))
                return (null, Invalid("This transition is not allowed. Rejection requires a reason of 1–500 characters while Submitted."));
            db.Entry(order).Property(o => o.RowVersion).OriginalValue = expected;
            try { await db.SaveChangesAsync(ct); }
            catch (DbUpdateConcurrencyException) { return (null, Stale()); }
            await db.Entry(order).Collection(o => o.Lines).LoadAsync(ct);
            await tx.CommitAsync(ct);
            return (Project(order), null);
        }, ct);
    }

    private async Task<(T? Value, OperationError? Error)> ExecuteAsync<T>(ClaimsPrincipal principal,
        Func<CafeDbContext, Task<(T? Value, OperationError? Error)>> action, CancellationToken ct) where T : class
    {
        try
        {
            var access = await staff.CheckCashierAsync(principal);
            if (access != StaffAccessResult.Allowed) return (null, new(access == StaffAccessResult.Unauthenticated
                ? OperationErrorKind.Unauthenticated : OperationErrorKind.Forbidden, "Current cashier access is required."));
            await using var db = await factory.CreateDbContextAsync(ct);
            return await action(db);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception) { return (null, new(OperationErrorKind.Unexpected, "Unable to complete this operation. Reload and try again.")); }
    }

    private static bool ValidStatus(string? value, out OrderStatus status) => Enum.TryParse(value, false, out status)
        && Enum.IsDefined(status) && status.ToString() == value;
    private static string Money(decimal value) => value.ToString("F2", CultureInfo.InvariantCulture);
    private static OperationError Invalid(string message) => new(OperationErrorKind.Invalid, message);
    private static OperationError Missing() => new(OperationErrorKind.NotFound, "This order could not be found.");
    private static OperationError Stale() => new(OperationErrorKind.Conflict, "The order changed. Reload before updating status.", "stale_order");
    private static PosOrderDetail Project(Order o) => new(o.Id, o.Reference, o.Origin.ToString(), o.Type.ToString(),
        o.TableLabel, o.CustomerName, o.CustomerPhone, o.Status.ToString(), o.RejectionReason, Money(o.Total),
        o.Currency, o.CreatedAtUtc, Convert.ToBase64String(o.RowVersion), o.Lines.OrderBy(l => l.ProductId)
            .Select(l => new OrderTrackingItem(l.ProductName, l.Quantity, Money(l.UnitPrice), Money(l.LineTotal))).ToArray()) { DeliveryAddress = o.DeliveryAddress };
}
