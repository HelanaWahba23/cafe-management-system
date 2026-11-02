namespace CafeManagement.Api.Domain;
public sealed class CustomerProfile
{
 public required string UserId { get; set; }
 public required string DisplayName { get; set; }
 public required string Phone { get; set; }
 public string? Address { get; set; }
 public byte[] RowVersion { get; private set; } = [];
}
