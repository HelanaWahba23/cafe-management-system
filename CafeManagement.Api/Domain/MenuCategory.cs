namespace CafeManagement.Api.Domain;

public sealed class MenuCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
