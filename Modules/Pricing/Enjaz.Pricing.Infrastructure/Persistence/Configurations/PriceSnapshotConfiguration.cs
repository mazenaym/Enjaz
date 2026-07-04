using Enjaz.Pricing.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Pricing.Infrastructure.Persistence.Configurations;

public sealed class PriceSnapshotConfiguration : IEntityTypeConfiguration<PriceSnapshot>
{
    public void Configure(EntityTypeBuilder<PriceSnapshot> builder)
    {
        builder.ToTable("price_snapshots");
        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.Id).HasColumnName("id");
        builder.Property(snapshot => snapshot.UserId).HasColumnName("user_id");
        builder.Property(snapshot => snapshot.ServiceCategoryId).HasColumnName("service_category_id");
        builder.Property(snapshot => snapshot.ServiceId).HasColumnName("service_id");
        builder.Property(snapshot => snapshot.ComplexityId).HasColumnName("complexity_id");
        builder.Property(snapshot => snapshot.PricingRuleId).HasColumnName("pricing_rule_id");
        builder.Property(snapshot => snapshot.CommissionSettingId).HasColumnName("commission_setting_id");
        builder.Property(snapshot => snapshot.VatSettingId).HasColumnName("vat_setting_id");
        builder.Property(snapshot => snapshot.DepositRuleId).HasColumnName("deposit_rule_id");
        builder.Property(snapshot => snapshot.BasePrice).HasColumnName("base_price").HasPrecision(12, 2);
        builder.Property(snapshot => snapshot.CommissionRate).HasColumnName("commission_rate").HasPrecision(5, 4);
        builder.Property(snapshot => snapshot.CommissionAmount).HasColumnName("commission_amount").HasPrecision(12, 2);
        builder.Property(snapshot => snapshot.VatRate).HasColumnName("vat_rate").HasPrecision(5, 4);
        builder.Property(snapshot => snapshot.VatAmount).HasColumnName("vat_amount").HasPrecision(12, 2);
        builder.Property(snapshot => snapshot.TotalAmount).HasColumnName("total_amount").HasPrecision(12, 2);
        builder.Property(snapshot => snapshot.TechnicianPayoutAmount).HasColumnName("technician_payout_amount").HasPrecision(12, 2);
        builder.Property(snapshot => snapshot.DepositAmount).HasColumnName("deposit_amount").HasPrecision(12, 2);
        builder.Property(snapshot => snapshot.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP");
        builder.Property(snapshot => snapshot.RequiresInspection).HasColumnName("requires_inspection");
        builder.Property(snapshot => snapshot.BreakdownJson).HasColumnName("breakdown_json").HasColumnType("jsonb");
        builder.Property(snapshot => snapshot.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(snapshot => snapshot.ExpiresAtUtc).HasColumnName("expires_at_utc");
        builder.HasIndex(snapshot => snapshot.UserId);
        builder.HasIndex(snapshot => snapshot.ServiceId);
        builder.HasIndex(snapshot => snapshot.CreatedAtUtc);
    }
}
