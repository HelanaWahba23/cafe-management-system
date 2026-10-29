using System.Globalization;
using System.Security.Claims;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

public sealed class AdminReportService(IDbContextFactory<CafeDbContext> factory, StaffAccess staff)
{
    public async Task<(AdminReport? Value, OperationError? Error)> GetAsync(ClaimsPrincipal principal,
        DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default)
    {
        try
        {
            var access = await staff.CheckAdminAsync(principal);
            if (access != StaffAccessResult.Allowed) return (null, new(access == StaffAccessResult.Unauthenticated
                ? OperationErrorKind.Unauthenticated : OperationErrorKind.Forbidden, "Current administrator access is required."));
            var last = to ?? DateOnly.FromDateTime(DateTime.UtcNow);
            if (last == DateOnly.MaxValue)
                return (null, Invalid());
            var first = from ?? last.AddDays(-Math.Min(6, last.DayNumber));
            if (first > last || last.DayNumber - first.DayNumber >= 366) return (null, Invalid());
            var start = first.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = last.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            await using var db = await factory.CreateDbContextAsync(ct);
            var orders = await db.Orders.AsNoTracking().Where(o => o.CreatedAtUtc >= start && o.CreatedAtUtc < end)
                .GroupBy(o => new { o.Status, o.Type, o.Origin })
                .Select(g => new { g.Key.Status, g.Key.Type, g.Key.Origin, Count = g.LongCount() }).ToArrayAsync(ct);
            var receipts = await db.PaymentSettlements.AsNoTracking()
                .Where(p => p.RecordedAtUtc >= start && p.RecordedAtUtc < end)
                .GroupBy(p => new { Day = p.RecordedAtUtc.Date, p.Method })
                .Select(g => new { g.Key.Day, g.Key.Method, Count = g.LongCount(), Amount = g.Sum(p => p.Amount) }).ToArrayAsync(ct);
            var cash = receipts.Where(r => r.Method == "Cash").OrderBy(r => r.Day).ToArray();
            var sandbox = receipts.Where(r => r.Method == "PaymobSandbox").ToArray();
            return (new(first, last, "EGP", orders.Sum(g => g.Count),
                Enum.GetValues<OrderStatus>().Select(status => new ReportCount(status.ToString(), orders.Where(g => g.Status == status).Sum(g => g.Count))).ToArray(),
                Enum.GetValues<OrderType>().Select(type => new ReportCount(type.ToString(), orders.Where(g => g.Type == type).Sum(g => g.Count))).ToArray(),
                Enum.GetValues<OrderOrigin>().Select(origin => new ReportCount(origin.ToString(), orders.Where(g => g.Origin == origin).Sum(g => g.Count))).ToArray(),
                cash.Sum(r => r.Count), Money(cash.Sum(r => r.Amount)), sandbox.Sum(r => r.Count), Money(sandbox.Sum(r => r.Amount)),
                cash.Select(r => new DailyCashReport(DateOnly.FromDateTime(r.Day), r.Count, Money(r.Amount))).ToArray()), null);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception) { return (null, new(OperationErrorKind.Unexpected, "Unable to load the report. Please try again.")); }
    }

    private static string Money(decimal value) => value.ToString("F2", CultureInfo.InvariantCulture);
    private static OperationError Invalid() => new(OperationErrorKind.Invalid,
        "Choose an inclusive UTC date range of 1–366 days with a valid end date.", "invalid_report_window");
}
