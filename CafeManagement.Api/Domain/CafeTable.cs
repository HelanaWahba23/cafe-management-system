namespace CafeManagement.Api.Domain;

public sealed class CafeTable
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string NormalizedLabel { get; set; }
    public Guid PublicCode { get; private set; } = Guid.NewGuid();
}
