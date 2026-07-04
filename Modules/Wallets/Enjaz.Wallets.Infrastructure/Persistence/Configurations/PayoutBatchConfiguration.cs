using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class PayoutBatchConfiguration : IEntityTypeConfiguration<PayoutBatch>
{
    public void Configure(EntityTypeBuilder<PayoutBatch> builder)
    {
        builder.ToTable("payout_batches");
        builder.HasKey(batch => batch.Id);
        builder.Property(batch => batch.Id).HasColumnName("id");
        builder.Property(batch => batch.BatchNumber).HasColumnName("batch_number").HasMaxLength(40).IsRequired();
        builder.Property(batch => batch.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(batch => batch.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP");
        builder.Property(batch => batch.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(batch => batch.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(batch => batch.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(batch => batch.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(batch => batch.BatchNumber).IsUnique();
    }
}
