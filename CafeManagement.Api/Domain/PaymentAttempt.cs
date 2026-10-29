namespace CafeManagement.Api.Domain;

public sealed class PaymentAttempt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public int IntegrationId { get; set; }
    public string State { get; set; } = "Creating";
    public bool IsBlocking { get; set; } = true;
    public long? ProviderOrderId { get; set; }
    public string? ProviderIntentionId { get; set; }
    public string? ProtectedClientSecret { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public byte[] RowVersion { get; private set; } = [];
}
