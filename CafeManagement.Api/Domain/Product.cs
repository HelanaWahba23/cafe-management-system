namespace CafeManagement.Api.Domain;

public sealed class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public required bool IsAvailable { get; set; }
    public MenuCategory Category { get; set; } = null!;
}
