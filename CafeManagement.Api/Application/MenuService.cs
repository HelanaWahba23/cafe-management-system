using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;
using CafeManagement.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Api.Application;

public sealed class MenuService(IDbContextFactory<CafeDbContext> factory, StaffAccess staff)
{
    public Task<(CategoryResponse[]? Value, OperationError? Error)> ListCategoriesAsync(ClaimsPrincipal principal,
        CancellationToken cancellationToken = default) => ExecuteAsync<CategoryResponse[]>(principal, async db =>
        (await db.MenuCategories.AsNoTracking().OrderBy(c => c.Id)
            .Select(c => new CategoryResponse(c.Id, c.Name)).ToArrayAsync(cancellationToken), null), cancellationToken);

    public Task<(CategoryResponse? Value, OperationError? Error)> GetCategoryAsync(ClaimsPrincipal principal, int id,
        CancellationToken cancellationToken = default) => ExecuteAsync<CategoryResponse>(principal, async db =>
    {
        if (id <= 0) return (null, InvalidId());
        var item = await db.MenuCategories.AsNoTracking().Where(c => c.Id == id)
            .Select(c => new CategoryResponse(c.Id, c.Name)).SingleOrDefaultAsync(cancellationToken);
        return item is null ? (null, Missing()) : (item, null);
    }, cancellationToken);

    public Task<(CategoryResponse? Value, OperationError? Error)> CreateCategoryAsync(ClaimsPrincipal principal,
        CategoryEditModel input, CancellationToken cancellationToken = default) => SaveCategoryAsync(principal, null, input, cancellationToken);

    public Task<(CategoryResponse? Value, OperationError? Error)> EditCategoryAsync(ClaimsPrincipal principal, int id,
        CategoryEditModel input, CancellationToken cancellationToken = default) => SaveCategoryAsync(principal, id, input, cancellationToken);

    private Task<(CategoryResponse? Value, OperationError? Error)> SaveCategoryAsync(ClaimsPrincipal principal,
        int? id, CategoryEditModel input, CancellationToken cancellationToken) => ExecuteAsync<CategoryResponse>(principal, async db =>
    {
        if (id is <= 0) return (null, InvalidId());
        var invalid = Validate(input);
        if (invalid is not null) return (null, invalid);
        var category = id is null ? new MenuCategory { Name = input.Name.Trim() }
            : await db.MenuCategories.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (category is null) return (null, Missing());
        category.Name = input.Name.Trim();
        if (id is null) db.MenuCategories.Add(category);
        await db.SaveChangesAsync(cancellationToken);
        return (new CategoryResponse(category.Id, category.Name), null);
    }, cancellationToken);

    public Task<(ProductDetails[]? Value, OperationError? Error)> ListProductsAsync(ClaimsPrincipal principal,
        CancellationToken cancellationToken = default) => ExecuteAsync<ProductDetails[]>(principal, async db =>
        (await db.Products.AsNoTracking().OrderBy(p => p.Id)
            .Select(p => new ProductDetails(p.Id, p.CategoryId, p.Name, p.Price, p.IsAvailable))
            .ToArrayAsync(cancellationToken), null), cancellationToken);

    public Task<(ProductDetails? Value, OperationError? Error)> GetProductAsync(ClaimsPrincipal principal, int id,
        CancellationToken cancellationToken = default) => ExecuteAsync<ProductDetails>(principal, async db =>
    {
        if (id <= 0) return (null, InvalidId());
        var item = await db.Products.AsNoTracking().Where(p => p.Id == id)
            .Select(p => new ProductDetails(p.Id, p.CategoryId, p.Name, p.Price, p.IsAvailable))
            .SingleOrDefaultAsync(cancellationToken);
        return item is null ? (null, Missing()) : (item, null);
    }, cancellationToken);

    public Task<(ProductDetails? Value, OperationError? Error)> CreateProductAsync(ClaimsPrincipal principal,
        ProductEditModel input, CancellationToken cancellationToken = default) => SaveProductAsync(principal, null, input, cancellationToken);

    public Task<(ProductDetails? Value, OperationError? Error)> EditProductAsync(ClaimsPrincipal principal, int id,
        ProductEditModel input, CancellationToken cancellationToken = default) => SaveProductAsync(principal, id, input, cancellationToken);

    private Task<(ProductDetails? Value, OperationError? Error)> SaveProductAsync(ClaimsPrincipal principal,
        int? id, ProductEditModel input, CancellationToken cancellationToken) => ExecuteAsync<ProductDetails>(principal, async db =>
    {
        if (id is <= 0) return (null, InvalidId());
        var invalid = Validate(input);
        if (invalid is not null) return (null, invalid);
        var product = id is null ? new Product { Name = input.Name.Trim(), IsAvailable = input.IsAvailable!.Value }
            : await db.Products.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null) return (null, Missing());
        if (!await db.MenuCategories.AnyAsync(c => c.Id == input.CategoryId, cancellationToken))
            return (null, new OperationError(OperationErrorKind.Invalid, "Choose an existing category.",
                FieldErrors: new Dictionary<string, string[]> { ["categoryId"] = ["Category does not exist."] }));
        product.Name = input.Name.Trim();
        product.CategoryId = input.CategoryId!.Value;
        product.Price = input.Price!.Value;
        product.IsAvailable = input.IsAvailable!.Value;
        if (id is null) db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return (new ProductDetails(product.Id, product.CategoryId, product.Name, product.Price, product.IsAvailable), null);
    }, cancellationToken);

    private async Task<(T? Value, OperationError? Error)> ExecuteAsync<T>(ClaimsPrincipal principal,
        Func<CafeDbContext, Task<(T? Value, OperationError? Error)>> operation, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var access = await staff.CheckAdminAsync(principal);
            if (access != StaffAccessResult.Allowed)
                return (null, new OperationError(access == StaffAccessResult.Unauthenticated
                    ? OperationErrorKind.Unauthenticated : OperationErrorKind.Forbidden, "Administrator access is required."));
            await using var db = await factory.CreateDbContextAsync(cancellationToken);
            return await operation(db);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception)
        {
            return (null, new OperationError(OperationErrorKind.Unexpected, "Unable to complete the menu operation."));
        }
    }

    private static OperationError? Validate(object input)
    {
        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(input, new ValidationContext(input), results, true)) return null;
        var fields = results.SelectMany(r => r.MemberNames.Select(name =>
            new { Name = char.ToLowerInvariant(name[0]) + name[1..], Message = r.ErrorMessage ?? "Invalid value." }))
            .GroupBy(r => r.Name).ToDictionary(g => g.Key, g => g.Select(r => r.Message).ToArray());
        return new OperationError(OperationErrorKind.Invalid, "Check the supplied values.", FieldErrors: fields);
    }

    private static OperationError InvalidId() => new(OperationErrorKind.Invalid, "Use a positive record ID.");
    private static OperationError Missing() => new(OperationErrorKind.NotFound, "The requested record was not found.");
}
