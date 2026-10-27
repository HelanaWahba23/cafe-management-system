using CafeManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Api.Infrastructure.Persistence.Configurations;

public sealed class CafeTableConfiguration : IEntityTypeConfiguration<CafeTable>
{
    public void Configure(EntityTypeBuilder<CafeTable> builder)
    {
        builder.ToTable("CafeTables", table =>
        {
            table.HasCheckConstraint("CK_CafeTables_Label_NotBlank", "LEN(LTRIM(RTRIM([Label]))) > 0");
            table.HasCheckConstraint("CK_CafeTables_NormalizedLabel_NotBlank", "LEN(LTRIM(RTRIM([NormalizedLabel]))) > 0");
        });
        builder.HasKey(table => table.Id);
        builder.Property(table => table.Id).UseIdentityColumn();
        builder.Property(table => table.Label).HasMaxLength(50).IsRequired();
        builder.Property(table => table.NormalizedLabel)
            .HasMaxLength(50)
            .UseCollation("Latin1_General_100_BIN2")
            .IsRequired();
        var publicCode = builder.Property(table => table.PublicCode).ValueGeneratedNever().IsRequired();
        publicCode.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.HasIndex(table => table.NormalizedLabel).IsUnique();
        builder.HasIndex(table => table.PublicCode).IsUnique();
    }
}
