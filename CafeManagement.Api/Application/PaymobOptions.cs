namespace CafeManagement.Api.Application;

public sealed class PaymobOptions
{
    public bool Enabled { get; set; }
    public string SecretKey { get; set; } = "";
    public string PublicKey { get; set; } = "";
    public string HmacSecret { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public int IntegrationId { get; set; }
    public string CallbackBaseUrl { get; set; } = "";

    public bool IsReady => Enabled && IsTestKey(SecretKey, "sk_test_") && IsTestKey(PublicKey, "pk_test_")
        && !string.IsNullOrWhiteSpace(HmacSecret) && !string.IsNullOrWhiteSpace(ApiKey) && IntegrationId > 0
        && Uri.TryCreate(CallbackBaseUrl, UriKind.Absolute, out var uri) && uri.Scheme == "https"
        && !uri.IsLoopback && uri.UserInfo.Length == 0 && uri.AbsolutePath == "/" && uri.Query.Length == 0 && uri.Fragment.Length == 0;

    private static bool IsTestKey(string? value, string prefix) => value?.StartsWith(prefix, StringComparison.Ordinal) == true
        || value?.StartsWith("egy_" + prefix, StringComparison.Ordinal) == true;
}
