using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using CafeManagement.Api.Domain;

namespace CafeManagement.Api.Contracts;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class OrderDraft : IValidatableObject
{
    private string customerName = string.Empty;
    private string customerPhone = string.Empty;

    public string OrderType { get; set; } = string.Empty;
    public Guid? TablePublicCode { get; set; }
    public string CustomerName { get => customerName; set => customerName = value?.Trim() ?? string.Empty; }
    public string CustomerPhone { get => customerPhone; set => customerPhone = value?.Trim() ?? string.Empty; }
    public string? DeliveryAddress { get; set; }
    public bool AddressConfirmed { get; set; }
    public OrderDraftItem?[]? Items { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => ValidateCore(false);

    internal IEnumerable<ValidationResult> ValidateCounter() => ValidateCore(true);

    private IEnumerable<ValidationResult> ValidateCore(bool optionalContacts)
    {
        if (OrderType is not (nameof(Domain.OrderType.DineIn) or nameof(Domain.OrderType.Pickup) or nameof(Domain.OrderType.Delivery)))
            yield return new ValidationResult("Choose dine-in, pickup or delivery.", [nameof(OrderType)]);
        if (OrderType == nameof(Domain.OrderType.DineIn) && (TablePublicCode is null || TablePublicCode == Guid.Empty))
            yield return new ValidationResult("Supply a valid table QR code for dine-in.", [nameof(TablePublicCode)]);
        if (OrderType != nameof(Domain.OrderType.DineIn) && TablePublicCode is not null)
            yield return new ValidationResult("Pickup must not include a table code.", [nameof(TablePublicCode)]);
        if (OrderType == nameof(Domain.OrderType.Delivery) && (string.IsNullOrWhiteSpace(DeliveryAddress) || DeliveryAddress.Trim().Length is < 8 or > 500 || !AddressConfirmed))
            yield return new ValidationResult("Enter and confirm a delivery address of 8–500 characters.", [nameof(DeliveryAddress)]);
        if (OrderType != nameof(Domain.OrderType.Delivery) && !string.IsNullOrEmpty(DeliveryAddress))
            yield return new ValidationResult("Only delivery orders may include an address.", [nameof(DeliveryAddress)]);
        if ((!optionalContacts && string.IsNullOrWhiteSpace(CustomerName)) || CustomerName.Length > 120)
            yield return new ValidationResult("Enter a name of 1–120 characters.", [nameof(CustomerName)]);
        if ((!optionalContacts && string.IsNullOrWhiteSpace(CustomerPhone)) || CustomerPhone.Length > 40)
            yield return new ValidationResult("Enter a phone number of 1–40 characters.", [nameof(CustomerPhone)]);

        if (Items is null || Items.Length == 0)
        {
            yield return new ValidationResult("Select at least one product.", [nameof(Items)]);
            yield break;
        }

        var productIds = new HashSet<int>();
        for (var index = 0; index < Items.Length; index++)
        {
            var item = Items[index];
            if (item is null)
            {
                yield return new ValidationResult("Supply a product and quantity.", [$"Items[{index}]"]);
                continue;
            }
            if (item.ProductId <= 0 || !productIds.Add(item.ProductId))
                yield return new ValidationResult("Supply a positive product ID once only.", [$"Items[{index}].ProductId"]);
            if (item.Quantity <= 0)
                yield return new ValidationResult("Enter a positive whole quantity.", [$"Items[{index}].Quantity"]);
        }
    }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonNumberHandling(JsonNumberHandling.Strict)]
public sealed class OrderDraftItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonNumberHandling(JsonNumberHandling.Strict)]
public sealed class ReviewedPrice
{
    public int ProductId { get; set; }
    public string UnitPrice { get; set; } = string.Empty;

    public bool TryReadUnitPrice(out decimal price)
    {
        price = 0;
        return UnitPrice is not null
            && Regex.IsMatch(UnitPrice, @"\A[0-9]+\.[0-9]{2}\z", RegexOptions.NonBacktracking)
            && decimal.TryParse(UnitPrice, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price)
            && price > 0 && price <= ProductEditModel.MaximumPrice;
    }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class OrderPlacementInput : IValidatableObject
{
    public OrderDraft? Draft { get; set; }
    public ReviewedPrice?[]? ReviewedPrices { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => ValidateCore(false);

    internal IEnumerable<ValidationResult> ValidateCounter() => ValidateCore(true);

    private IEnumerable<ValidationResult> ValidateCore(bool optionalContacts)
    {
        if (Draft is null)
            yield return new ValidationResult("Supply the order draft.", [nameof(Draft)]);
        else
            foreach (var issue in optionalContacts ? Draft.ValidateCounter() : Draft.Validate(new ValidationContext(Draft)))
                yield return new ValidationResult(issue.ErrorMessage, issue.MemberNames.Select(name => $"Draft.{name}"));

        if (ReviewedPrices is null || ReviewedPrices.Length == 0)
        {
            yield return new ValidationResult("Review every selected product price before placing the order.", [nameof(ReviewedPrices)]);
            yield break;
        }

        var reviewedIds = new HashSet<int>();
        for (var index = 0; index < ReviewedPrices.Length; index++)
        {
            var reviewed = ReviewedPrices[index];
            if (reviewed is null)
            {
                yield return new ValidationResult("Supply a reviewed product price.", [$"ReviewedPrices[{index}]"]);
                continue;
            }
            if (reviewed.ProductId <= 0 || !reviewedIds.Add(reviewed.ProductId))
                yield return new ValidationResult("Supply each positive reviewed product ID once only.", [$"ReviewedPrices[{index}].ProductId"]);
            if (!reviewed.TryReadUnitPrice(out _))
                yield return new ValidationResult("Enter a positive EGP price with exactly two decimal places within the supported range.", [$"ReviewedPrices[{index}].UnitPrice"]);
        }

        if (Draft?.Items is not null && !reviewedIds.SetEquals(Draft.Items.Where(item => item is not null).Select(item => item!.ProductId)))
            yield return new ValidationResult("Reviewed prices must match the selected products exactly.", [nameof(ReviewedPrices)]);
    }
}

// Explicit public projections: no internal order/table IDs, contacts, credentials or persistence entities.
public sealed record OrderReviewTable(Guid PublicCode, string Label);
public sealed record OrderReviewItem(int ProductId, string ProductName, int Quantity, string UnitPrice, string LineTotal);
public sealed record OrderReview(string OrderType, OrderReviewTable? Table, OrderReviewItem[] Items, string Total)
{
    public string Currency => "EGP";
}

public sealed record OrderTrackingItem(string ProductName, int Quantity, string UnitPrice, string LineTotal);
public sealed record OrderTracking(Guid Reference, string OrderType, string? TableLabel, OrderTrackingItem[] Items,
    string Total, string Status, string? RejectionReason, DateTime CreatedAtUtc)
{
    public string Currency => "EGP";
    public string? DeliveryAddress { get; init; }
}

public sealed record OrderPlacementResult(OrderTracking Order)
{
    public string TrackingPath => "/orders/track";
}

// Service outcome; endpoints serialize only Value or a safe ProblemDetails projection.
public sealed record OrderPlacementOutcome(OrderPlacementResult? Value, Application.OperationError? Error,
    bool Created = false, OrderReview? Review = null);
