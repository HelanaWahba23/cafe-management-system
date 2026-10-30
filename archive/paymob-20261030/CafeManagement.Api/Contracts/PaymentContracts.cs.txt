using System.Text.Json.Serialization;

namespace CafeManagement.Api.Contracts;

public sealed record PaymentStatus(string State, string? Method, string Total, string Currency,
    DateTime? RecordedAtUtc, bool OnlineAvailable);
public sealed record StaffPaymentStatus(PaymentStatus Payment, string RowVersion, string? RecordedByUserId);
public sealed record OnlinePaymentOutcome(PaymentStatus? Status, string? CheckoutUrl, Application.OperationError? Error);

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class CashPaymentInput
{
    public string RowVersion { get; set; } = string.Empty;
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class PaymentActionInput;
