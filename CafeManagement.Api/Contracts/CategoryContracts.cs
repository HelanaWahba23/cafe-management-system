using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Api.Contracts;

public sealed class CategoryInput : CategoryEditModel;

public class CategoryEditModel : IValidatableObject
{
    public string Name { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length > 120)
            yield return new ValidationResult("Enter a name of 1–120 characters.", [nameof(Name)]);
    }
}

public sealed record CategoryResponse(int Id, string Name);
