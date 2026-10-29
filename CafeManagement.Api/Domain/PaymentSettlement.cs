namespace CafeManagement.Api.Domain;

public sealed class PaymentSettlement
{
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public required string Method { get; set; }
    public string? RecordedByUserId { get; set; }
    public long? ProviderTransactionId { get; set; }
    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;
}
