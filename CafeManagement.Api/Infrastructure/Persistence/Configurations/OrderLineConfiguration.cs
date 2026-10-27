using CafeManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Api.Infrastructure.Persistence.Configurations;

public sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines", table =>
        {
            table.HasCheckConstraint("CK_OrderLines_ProductName_NotBlank", "LEN(LTRIM(RTRIM([ProductName]))) > 0");
            table.HasCheckConstraint("CK_OrderLines_Quantity_Positive", "[Quantity] > 0");
            table.HasCheckConstraint("CK_OrderLines_UnitPrice_Positive", "[UnitPrice] > 0");
            table.HasCheckConstraint("CK_OrderLines_LineTotal_Positive", "[LineTotal] > 0");
            table.HasCheckConstraint("CK_OrderLines_LineTotal_Arithmetic",
                "CONVERT(decimal(29,2), [LineTotal]) = CONVERT(decimal(18,2), [UnitPrice]) * CONVERT(decimal(10,0), [Quantity])");
        });

        builder.HasKey(line => line.Id);
        builder.Property(line => line.Id).UseIdentityColumn();
        builder.Property(line => line.ProductName).HasMaxLength(120).IsRequired();
        builder.Property(line => line.Quantity).IsRequired();
        builder.Property(line => line.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(line => line.LineTotal).HasPrecision(18, 2).IsRequired();

        builder.HasOne(line => line.Order)
            .WithMany(order => order.Lines)
            .HasForeignKey(line => line.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(line => line.Product)
            .WithMany()
            .HasForeignKey(line => line.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(line => new { line.OrderId, line.ProductId }).IsUnique();
        builder.HasIndex(line => line.ProductId);
    }
}
