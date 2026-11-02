using CafeManagement.Api.Contracts;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

public sealed class PublicMenuService(IDbContextFactory<CafeDbContext> factory, ILogger<PublicMenuService> logger)
{
    public async Task<(PublicMenuDetails? Value, OperationError? Error)> GetAsync(string? publicCode,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(publicCode, out var code))
            return (null, new OperationError(OperationErrorKind.Invalid, "This table link is invalid. Check the table QR code."));

        try
        {
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            var table = await db.CafeTables.AsNoTracking().Where(t => t.PublicCode == code)
                .Select(t => new PublicMenuTable(t.Label, t.PublicCode)).SingleOrDefaultAsync(cancellationToken);
            if (table is null)
                return (null, new OperationError(OperationErrorKind.NotFound, "This table could not be found. Check the table QR code."));

            var categories = await CategoriesAsync(db, cancellationToken);
            return (new PublicMenuDetails(table, "EGP", categories), null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to load the table menu.");
            return (null, new OperationError(OperationErrorKind.Unexpected, "Unable to load the menu. Please try again."));
        }
    }

    public async Task<(PickupMenuDetails? Value, OperationError? Error)> GetPickupAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            return (new PickupMenuDetails("EGP", await CategoriesAsync(db, cancellationToken)), null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to load the pickup menu.");
            return (null, new OperationError(OperationErrorKind.Unexpected, "Unable to load the menu. Please try again."));
        }
    }

    // Shared available-only projection, retaining explicit empty categories and stable ordering.
    private static Task<PublicMenuCategory[]> CategoriesAsync(CafeDbContext db, CancellationToken cancellationToken) =>
        db.MenuCategories.AsNoTracking().Where(c => c.IsVisible).OrderBy(c => c.Id)
            .Select(c => new PublicMenuCategory(c.Id, c.Name,
                c.Products.Where(p => p.IsAvailable).OrderBy(p => p.Id)
                    .Select(p => new PublicMenuProduct(p.Id, p.Name, p.Price)).ToArray()))
            .ToArrayAsync(cancellationToken);
}
