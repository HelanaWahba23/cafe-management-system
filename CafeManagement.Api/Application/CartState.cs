using CafeManagement.Api.Contracts;
using CafeManagement.Api.Domain;

namespace CafeManagement.Api.Application;

// In-memory state for one interactive menu root; never holds EF entities or credentials.
public sealed class CartState
{
    private readonly List<CartItem> items = [];
    public IReadOnlyList<CartItem> Items => items.AsReadOnly();
    public OrderType Type { get; private set; } = OrderType.Pickup;
    public Guid? TablePublicCode { get; private set; }
    public decimal Total => items.Sum(item => item.LineTotal);

    public void SetContext(OrderType type, Guid? tablePublicCode)
    {
        if ((type == OrderType.DineIn && (tablePublicCode is null || tablePublicCode == Guid.Empty))
            || (type == OrderType.Pickup && tablePublicCode is not null)
            || !Enum.IsDefined(type))
            throw new ArgumentException("Supply the correct order type and table context.");

        if (Type != type || TablePublicCode != tablePublicCode) items.Clear();
        Type = type;
        TablePublicCode = tablePublicCode;
    }

    public bool TryAdd(PublicMenuProduct product, out string? error)
    {
        var existing = items.Find(item => item.ProductId == product.Id);
        if (existing?.Quantity == int.MaxValue)
        {
            error = "This quantity exceeds the supported range.";
            return false;
        }
        return TryReplace(new CartItem(product.Id, product.Name, (existing?.Quantity ?? 0) + 1, product.Price), out error);
    }

    public bool TryUpdateQuantity(int productId, int quantity, out string? error)
    {
        var item = items.Find(item => item.ProductId == productId);
        if (item is null)
        {
            error = "This product is no longer in the cart.";
            return false;
        }
        return TryReplace(item with { Quantity = quantity }, out error);
    }

    public void Remove(int productId) => items.RemoveAll(item => item.ProductId == productId);

    private bool TryReplace(CartItem candidate, out string? error)
    {
        error = null;
        if (candidate.ProductId <= 0 || string.IsNullOrWhiteSpace(candidate.ProductName)
            || candidate.UnitPrice <= 0 || candidate.UnitPrice > ProductEditModel.MaximumPrice
            || decimal.Round(candidate.UnitPrice, 2) != candidate.UnitPrice)
        {
            error = "This product's displayed price or details are invalid. Reload the menu.";
            return false;
        }
        if (candidate.Quantity <= 0)
        {
            error = "Enter a positive whole quantity, or use Remove.";
            return false;
        }

        try
        {
            var total = candidate.LineTotal;
            foreach (var item in items.Where(item => item.ProductId != candidate.ProductId))
                total = checked(total + item.LineTotal);
            if (candidate.LineTotal > ProductEditModel.MaximumPrice || total > ProductEditModel.MaximumPrice)
                throw new OverflowException();
        }
        catch (OverflowException)
        {
            error = "This quantity or total exceeds the supported monetary range.";
            return false;
        }

        var index = items.FindIndex(item => item.ProductId == candidate.ProductId);
        if (index < 0) items.Add(candidate);
        else items[index] = candidate;
        return true;
    }
}

public sealed record CartItem(int ProductId, string ProductName, int Quantity, decimal UnitPrice)
{
    public decimal LineTotal => checked(UnitPrice * Quantity);
}
