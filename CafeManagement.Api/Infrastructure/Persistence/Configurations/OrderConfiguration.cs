using CafeManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Api.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", table =>
        {
            table.HasCheckConstraint("CK_Orders_Type", "[Type] IN (N'DineIn', N'Pickup')");
            table.HasCheckConstraint("CK_Orders_Status",
                "[Status] IN (N'Submitted', N'Preparing', N'Ready', N'Completed', N'Rejected')");
            table.HasCheckConstraint("CK_Orders_Currency", "[Currency] = 'EGP'");
            table.HasCheckConstraint("CK_Orders_Total_Positive", "[Total] > 0");
            table.HasCheckConstraint("CK_Orders_CustomerName_NotBlank", "LEN(LTRIM(RTRIM([CustomerName]))) > 0");
            table.HasCheckConstraint("CK_Orders_CustomerPhone_NotBlank", "LEN(LTRIM(RTRIM([CustomerPhone]))) > 0");
            table.HasCheckConstraint("CK_Orders_TableContext",
                "([Type] = N'DineIn' AND [TableId] IS NOT NULL AND [TableLabel] IS NOT NULL AND LEN(LTRIM(RTRIM([TableLabel]))) > 0) " +
                "OR ([Type] = N'Pickup' AND [TableId] IS NULL AND [TableLabel] IS NULL)");
            table.HasCheckConstraint("CK_Orders_RejectionReason",
                "([Status] = N'Rejected' AND [RejectionReason] IS NOT NULL AND LEN(LTRIM(RTRIM([RejectionReason]))) > 0) " +
                "OR ([Status] <> N'Rejected' AND [RejectionReason] IS NULL)");
        });

        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id).UseIdentityColumn();
        builder.Property(order => order.Reference).ValueGeneratedNever().IsRequired();
        builder.Property(order => order.CapabilityHash).HasColumnType("binary(32)").HasMaxLength(32).IsFixedLength().IsRequired();
        builder.Property(order => order.IntentHash).HasColumnType("binary(32)").HasMaxLength(32).IsFixedLength().IsRequired();
        builder.Property(order => order.Type).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(order => order.TableLabel).HasMaxLength(120);
        builder.Property(order => order.CustomerName).HasMaxLength(120).IsRequired();
        builder.Property(order => order.CustomerPhone).HasMaxLength(40).IsRequired();
        builder.Property(order => order.Currency).HasColumnType("char(3)").HasMaxLength(3).IsFixedLength().IsUnicode(false).IsRequired();
        builder.Property(order => order.Total).HasPrecision(18, 2).IsRequired();
        builder.Property(order => order.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(order => order.RejectionReason).HasMaxLength(500);
        builder.Property(order => order.CreatedAtUtc).HasColumnType("datetime2").IsRequired();
        builder.Property(order => order.RowVersion).IsRowVersion();

        builder.HasOne(order => order.Table)
            .WithMany()
            .HasForeignKey(order => order.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(order => order.Reference).IsUnique();
        builder.HasIndex(order => order.CapabilityHash).IsUnique();
        builder.HasIndex(order => order.TableId);
        builder.HasIndex(order => new { order.Status, order.CreatedAtUtc });
    }
}
