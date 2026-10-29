using System.Text.Json.Serialization;

namespace CafeManagement.Api.Contracts;

public sealed record PosOrderSummary(long Id, Guid Reference, string Origin, string OrderType,
    string? TableLabel, string Status, string Total, DateTime CreatedAtUtc);
public sealed record PosOrderPage(PosOrderSummary[] Items, int TotalCount, int Page, int PageSize);
public sealed record PosOrderDetail(long Id, Guid Reference, string Origin, string OrderType,
    string? TableLabel, string? CustomerName, string? CustomerPhone, string Status,
    string? RejectionReason, string Total, string Currency, DateTime CreatedAtUtc,
    string RowVersion, OrderTrackingItem[] Items);

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class PosStatusInput
{
    public string Status { get; set; } = string.Empty;
    public string RowVersion { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class CounterOrderInput
{
    public string AttemptKey { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public Guid? TablePublicCode { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public OrderDraftItem?[]? Items { get; set; } = [];
    public ReviewedPrice?[]? ReviewedPrices { get; set; } = [];

    internal OrderPlacementInput Snapshot() => new()
    {
        Draft = new() { OrderType = OrderType, TablePublicCode = TablePublicCode,
            CustomerName = CustomerName ?? "", CustomerPhone = CustomerPhone ?? "",
            Items = Items?.Select(i => i is null ? null : new OrderDraftItem { ProductId = i.ProductId, Quantity = i.Quantity }).ToArray() },
        ReviewedPrices = ReviewedPrices?.Select(p => p is null ? null : new ReviewedPrice { ProductId = p.ProductId, UnitPrice = p.UnitPrice }).ToArray()
    };
}

public sealed record PosTable(Guid PublicCode, string Label);
public sealed record CounterPlacement(Guid Reference, long Id);
public sealed record CounterOutcome(CounterPlacement? Value, Application.OperationError? Error, bool Created = false, OrderReview? Review = null);
