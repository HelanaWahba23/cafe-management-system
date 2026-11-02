using CafeManagement.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CafeManagement.Api.Infrastructure.Persistence.Configurations;
public sealed class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
 public void Configure(EntityTypeBuilder<CustomerProfile> b){b.HasKey(p=>p.UserId);b.Property(p=>p.DisplayName).HasMaxLength(120).IsRequired();b.Property(p=>p.Phone).HasMaxLength(40).IsRequired();b.Property(p=>p.Address).HasMaxLength(500);b.Property(p=>p.RowVersion).IsRowVersion();b.HasOne<IdentityUser>().WithOne().HasForeignKey<CustomerProfile>(p=>p.UserId).OnDelete(DeleteBehavior.Restrict);}
}
