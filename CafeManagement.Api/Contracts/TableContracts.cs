using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Api.Contracts;

public sealed class TableInput : TableEditModel;

public class TableEditModel : IValidatableObject
{
    public string Label { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Label) || Label.Trim().Length > 50
            || Label.Trim().ToUpperInvariant().Length > 50)
            yield return new ValidationResult("Enter a table label of 1–50 characters.", [nameof(Label)]);
    }
}

public sealed record TableResponse(int Id, string Label, Guid PublicCode, string MenuUrl);
