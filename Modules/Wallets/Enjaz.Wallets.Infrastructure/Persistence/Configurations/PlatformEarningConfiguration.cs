using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class PlatformEarningConfiguration : IEntityTypeConfiguration<PlatformEarning>
{
    public void Configure(EntityTypeBuilder<PlatformEarning> builder)
    {
        builder.ToTable("platform_earnings");
        builder.HasKey(earning => earning.Id);
        builder.Property(earning => earning.Id).HasColumnName("id");
        builder.Property(earning => earning.PaymentId).HasColumnName("payment_id");
        builder.Property(earning => earning.JobId).HasColumnName("job_id");
        builder.Property(earning => earning.PriceSnapshotId).HasColumnName("price_snapshot_id");
        builder.Property(earning => earning.CustomerUserId).HasColumnName("customer_user_id");
        builder.Property(earning => earning.TechnicianId).HasColumnName("technician_id");
        builder.Property(earning => earning.TechnicianUserId).HasColumnName("technician_user_id");
        builder.Property(earning => earning.BasePrice).HasColumnName("base_price").HasPrecision(18, 2);
        builder.Property(earning => earning.CommissionRate).HasColumnName("commission_rate").HasPrecision(9, 4);
        builder.Property(earning => earning.CommissionAmount).HasColumnName("commission_amount").HasPrecision(18, 2);
        builder.Property(earning => earning.VatRate).HasColumnName("vat_rate").HasPrecision(9, 4);
        builder.Property(earning => earning.VatAmount).HasColumnName("vat_amount").HasPrecision(18, 2);
        builder.Property(earning => earning.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2);
        builder.Property(earning => earning.DepositAmount).HasColumnName("deposit_amount").HasPrecision(18, 2);
        builder.Property(earning => earning.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP");
        builder.Property(earning => earning.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
        builder.Property(earning => earning.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(earning => earning.PaymentId).IsUnique();
        builder.HasIndex(earning => earning.JobId);
    }
}
