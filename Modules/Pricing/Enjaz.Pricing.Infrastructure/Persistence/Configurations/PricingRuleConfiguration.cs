using Enjaz.Pricing.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Pricing.Infrastructure.Persistence.Configurations;

public sealed class PricingRuleConfiguration : IEntityTypeConfiguration<PricingRule>
{
    public void Configure(EntityTypeBuilder<PricingRule> builder)
    {
        builder.ToTable("pricing_rules");
        builder.HasKey(rule => rule.Id);
        builder.Property(rule => rule.Id).HasColumnName("id");
        builder.Property(rule => rule.ServiceCategoryId).HasColumnName("service_category_id");
        builder.Property(rule => rule.ServiceId).HasColumnName("service_id");
        builder.Property(rule => rule.ComplexityId).HasColumnName("complexity_id");
        builder.Property(rule => rule.BasePrice).HasColumnName("base_price").HasPrecision(12, 2);
        builder.Property(rule => rule.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP");
        builder.Property(rule => rule.RequiresInspection).HasColumnName("requires_inspection").HasDefaultValue(false);
        builder.Property(rule => rule.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(rule => rule.EffectiveFromUtc).HasColumnName("effective_from_utc");
        builder.Property(rule => rule.EffectiveToUtc).HasColumnName("effective_to_utc");
        builder.Property(rule => rule.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(rule => rule.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(rule => new { rule.ServiceId, rule.ComplexityId, rule.IsActive });
        builder.HasIndex(rule => new { rule.ServiceId, rule.ComplexityId })
            .IsUnique()
            .HasFilter("is_active = TRUE AND effective_to_utc IS NULL");
        builder.HasIndex(rule => rule.EffectiveFromUtc);
        builder.HasData(PricingSeed.PricingRules);
    }
}
