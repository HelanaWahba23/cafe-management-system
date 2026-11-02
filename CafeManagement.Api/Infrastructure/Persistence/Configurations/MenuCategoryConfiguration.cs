using CafeManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Api.Infrastructure.Persistence.Configurations;

public sealed class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.ToTable("MenuCategories", table =>
            table.HasCheckConstraint("CK_MenuCategories_Name_NotBlank", "LEN(LTRIM(RTRIM([Name]))) > 0"));
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).UseIdentityColumn();
        builder.Property(category => category.Name).HasMaxLength(120).IsRequired();
        builder.Property(category => category.IsVisible).HasDefaultValue(true);
    }
}
