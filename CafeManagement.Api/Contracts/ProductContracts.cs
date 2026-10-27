using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CafeManagement.Api.Contracts;

public sealed class ProductInput : IValidatableObject
{
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string Price { get; set; } = string.Empty;
    public bool? IsAvailable { get; set; }

    public ProductEditModel ToEditModel() => new()
    {
        Name = Name, CategoryId = CategoryId, IsAvailable = IsAvailable,
        Price = TryReadPrice(Price, out var price) ? price : null
    };

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var issue in ToEditModel().Validate(validationContext)) yield return issue;
    }

    private static bool TryReadPrice(string? text, out decimal price)
    {
        price = 0;
        return text is not null && Regex.IsMatch(text, @"\A[0-9]+(?:\.[0-9]{1,2})?\z")
            && decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price);
    }
}

public sealed class ProductEditModel : IValidatableObject
{
    public const decimal MaximumPrice = 9999999999999999.99m;
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public decimal? Price { get; set; }
    public bool? IsAvailable { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length > 120)
            yield return new ValidationResult("Enter a name of 1–120 characters.", [nameof(Name)]);
        if (CategoryId is null or <= 0)
            yield return new ValidationResult("Choose an existing category.", [nameof(CategoryId)]);
        if (Price is null or <= 0 or > MaximumPrice || ((decimal.GetBits(Price.Value)[3] >> 16) & 0xff) > 2)
            yield return new ValidationResult("Enter a positive price with at most two decimal places within the supported range.", [nameof(Price)]);
        if (IsAvailable is null)
            yield return new ValidationResult("Specify product availability.", [nameof(IsAvailable)]);
    }
}

public sealed record ProductDetails(int Id, int CategoryId, string Name, decimal Price, bool IsAvailable);

public sealed record ProductResponse(int Id, int CategoryId, string Name, string Price, bool IsAvailable, string Currency)
{
    public static ProductResponse From(ProductDetails product) => new(product.Id, product.CategoryId,
        product.Name, product.Price.ToString("F2", CultureInfo.InvariantCulture), product.IsAvailable, "EGP");
}
