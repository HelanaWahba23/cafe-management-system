namespace CafeManagement.Api.Domain;

public sealed class Order
{
    public long Id { get; set; }
    public Guid Reference { get; private set; } = Guid.NewGuid();
    public required byte[] CapabilityHash { get; set; }
    public required byte[] IntentHash { get; set; }
    public required OrderType Type { get; set; }
    public OrderOrigin Origin { get; set; } = OrderOrigin.Customer;
    public string? CreatedByUserId { get; set; }
    public long PaymentRevision { get; set; }
    public int? TableId { get; set; }
    public CafeTable? Table { get; set; }
    public string? TableLabel { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string Currency { get; private set; } = "EGP";
    public decimal Total { get; set; }
    public OrderStatus Status { get; internal set; } = OrderStatus.Submitted;
    public string? RejectionReason { get; internal set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public byte[] RowVersion { get; private set; } = [];
    public ICollection<OrderLine> Lines { get; } = new List<OrderLine>();
}
