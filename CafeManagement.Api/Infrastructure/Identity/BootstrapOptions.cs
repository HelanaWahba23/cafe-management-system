using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Api.Infrastructure.Identity;

// Bind only for the explicit operator bootstrap command. Never log these values.
public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required, RegularExpression("^(Admin|Cashier)$")]
    public string Role { get; set; } = string.Empty;
}
