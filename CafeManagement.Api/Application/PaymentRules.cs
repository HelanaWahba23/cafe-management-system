using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

internal static class PaymentRules
{
    // Caller owns a short transaction. All payment/status writers lock the order first.
    internal static Task<Order?> LockOrderAsync(CafeDbContext db, long id, CancellationToken ct) =>
        db.Orders.FromSqlInterpolated($"SELECT * FROM [Orders] WITH (UPDLOCK,HOLDLOCK) WHERE [Id] = {id}")
            .SingleOrDefaultAsync(ct);

    internal static async Task<bool> BlocksRejectionAsync(CafeDbContext db, long id, CancellationToken ct) =>
        await db.PaymentSettlements.AnyAsync(p => p.OrderId == id, ct)
        || await db.PaymentAttempts.AnyAsync(p => p.OrderId == id && p.IsBlocking, ct);
}
