using System.ComponentModel.DataAnnotations;

namespace CafeManagement.Api.Application;

// Bind from trusted server configuration, never from the request Host header.
public sealed class PublicMenuOptions : IValidatableObject
{
    public string PublicWebBaseUrl { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(PublicWebBaseUrl)
            || PublicWebBaseUrl != PublicWebBaseUrl.Trim()
            || !Uri.TryCreate(PublicWebBaseUrl, UriKind.Absolute, out var origin)
            || origin.Scheme != Uri.UriSchemeHttps
            || string.IsNullOrEmpty(origin.Host)
            || !string.IsNullOrEmpty(origin.UserInfo)
            || origin.AbsolutePath != "/"
            || !string.IsNullOrEmpty(origin.Query)
            || !string.IsNullOrEmpty(origin.Fragment))
        {
            yield return new ValidationResult(
                "PublicWebBaseUrl must be an absolute HTTPS origin without credentials, a path, query or fragment.",
                [nameof(PublicWebBaseUrl)]);
        }
    }
}
