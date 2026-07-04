using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class PayoutBatchItemConfiguration : IEntityTypeConfiguration<PayoutBatchItem>
{
    public void Configure(EntityTypeBuilder<PayoutBatchItem> builder)
    {
        builder.ToTable("payout_batch_items");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.PayoutBatchId).HasColumnName("payout_batch_id");
        builder.Property(item => item.TechnicianId).HasColumnName("technician_id");
        builder.Property(item => item.TechnicianUserId).HasColumnName("technician_user_id");
        builder.Property(item => item.WalletId).HasColumnName("wallet_id");
        builder.Property(item => item.Amount).HasColumnName("amount").HasPrecision(18, 2);
        builder.Property(item => item.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP");
        builder.Property(item => item.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(item => item.FailureReason).HasColumnName("failure_reason").HasMaxLength(500);
        builder.Property(item => item.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(item => item.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(item => item.PayoutBatchId);
        builder.HasIndex(item => item.TechnicianId);
    }
}
