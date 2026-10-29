using CafeManagement.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeManagement.Api.Infrastructure.Persistence.Configurations;

public sealed class PaymentSettlementConfiguration : IEntityTypeConfiguration<PaymentSettlement>
{
    public void Configure(EntityTypeBuilder<PaymentSettlement> b)
    {
        b.ToTable("PaymentSettlements", t =>
        {
            t.HasCheckConstraint("CK_PaymentSettlements_Amount", "[Amount] > 0 AND [Currency] = 'EGP'");
            t.HasCheckConstraint("CK_PaymentSettlements_Method", "([Method] = N'Cash' AND [RecordedByUserId] IS NOT NULL AND [ProviderTransactionId] IS NULL) OR ([Method] = N'PaymobSandbox' AND [RecordedByUserId] IS NULL AND [ProviderTransactionId] IS NOT NULL AND [ProviderTransactionId] > 0)");
        });
        b.HasKey(p => p.OrderId);
        b.Property(p => p.OrderId).ValueGeneratedNever();
        b.Property(p => p.Amount).HasPrecision(18, 2);
        b.Property(p => p.Currency).HasColumnType("char(3)").IsUnicode(false);
        b.Property(p => p.Method).HasMaxLength(24);
        b.HasOne<Order>().WithOne().HasForeignKey<PaymentSettlement>(p => p.OrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<IdentityUser>().WithMany().HasForeignKey(p => p.RecordedByUserId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(p => p.ProviderTransactionId).IsUnique().HasFilter("[ProviderTransactionId] IS NOT NULL");
        b.HasIndex(p => p.RecordedAtUtc);
    }
}

public sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> b)
    {
        b.ToTable("PaymentAttempts", t =>
        {
            t.HasCheckConstraint("CK_PaymentAttempts_Amount", "[Amount] > 0 AND [Currency] = 'EGP' AND [IntegrationId] > 0");
            t.HasCheckConstraint("CK_PaymentAttempts_State", "([State] IN (N'Creating', N'Pending', N'Uncertain') AND [IsBlocking] = 1) OR ([State] IN (N'Failed', N'Succeeded') AND [IsBlocking] = 0)");
            t.HasCheckConstraint("CK_PaymentAttempts_ProviderOrder", "[ProviderOrderId] IS NULL OR [ProviderOrderId] > 0");
        });
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).ValueGeneratedNever();
        b.Property(p => p.Amount).HasPrecision(18, 2);
        b.Property(p => p.Currency).HasColumnType("char(3)").IsUnicode(false);
        b.Property(p => p.State).HasMaxLength(16);
        b.Property(p => p.ProviderIntentionId).HasMaxLength(120);
        b.Property(p => p.ProtectedClientSecret).HasMaxLength(4096);
        b.Property(p => p.RowVersion).IsRowVersion();
        b.HasOne<Order>().WithMany().HasForeignKey(p => p.OrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(p => p.OrderId).IsUnique().HasFilter("[IsBlocking] = 1");
        b.HasIndex(p => p.ProviderOrderId).IsUnique().HasFilter("[ProviderOrderId] IS NOT NULL");
    }
}
