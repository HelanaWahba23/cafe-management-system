using System.Globalization;

namespace CafeManagement.Api.Contracts;

public sealed record PublicMenuTable(string Label, Guid PublicCode);
public sealed record PublicMenuProduct(int Id, string Name, decimal Price);
public sealed record PublicMenuCategory(int Id, string Name, PublicMenuProduct[] Products);
public sealed record PublicMenuDetails(PublicMenuTable Table, string Currency, PublicMenuCategory[] Categories);
public sealed record PickupMenuDetails(string Currency, PublicMenuCategory[] Categories);

public sealed record PublicMenuProductResponse(int Id, string Name, string Price);
public sealed record PublicMenuCategoryResponse(int Id, string Name, PublicMenuProductResponse[] Products);
public sealed record PublicMenuResponse(PublicMenuTable Table, string Currency, PublicMenuCategoryResponse[] Categories)
{
    public static PublicMenuResponse From(PublicMenuDetails menu) => new(menu.Table, menu.Currency,
        menu.Categories.Select(category => new PublicMenuCategoryResponse(category.Id, category.Name,
            category.Products.Select(product => new PublicMenuProductResponse(product.Id, product.Name,
                product.Price.ToString("F2", CultureInfo.InvariantCulture))).ToArray())).ToArray());
}

public sealed record PickupMenuResponse(string Currency, PublicMenuCategoryResponse[] Categories)
{
    public static PickupMenuResponse From(PickupMenuDetails menu) => new(menu.Currency,
        menu.Categories.Select(category => new PublicMenuCategoryResponse(category.Id, category.Name,
            category.Products.Select(product => new PublicMenuProductResponse(product.Id, product.Name,
                product.Price.ToString("F2", CultureInfo.InvariantCulture))).ToArray())).ToArray());
}
