using Enjaz.Pricing.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Pricing.Infrastructure.Persistence.Configurations;

public sealed class DepositRuleConfiguration : IEntityTypeConfiguration<DepositRule>
{
    public void Configure(EntityTypeBuilder<DepositRule> builder)
    {
        builder.ToTable("deposit_rules");
        builder.HasKey(rule => rule.Id);
        builder.Property(rule => rule.Id).HasColumnName("id");
        builder.Property(rule => rule.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(rule => rule.DepositType).HasColumnName("deposit_type").HasMaxLength(50).IsRequired();
        builder.Property(rule => rule.DepositValue).HasColumnName("deposit_value").HasPrecision(12, 4);
        builder.Property(rule => rule.MinimumDeposit).HasColumnName("minimum_deposit").HasPrecision(12, 2);
        builder.Property(rule => rule.MaximumDeposit).HasColumnName("maximum_deposit").HasPrecision(12, 2);
        builder.Property(rule => rule.IsDefault).HasColumnName("is_default");
        builder.Property(rule => rule.IsActive).HasColumnName("is_active");
        builder.Property(rule => rule.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(rule => rule.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(rule => rule.IsDefault)
            .IsUnique()
            .HasFilter("is_default = TRUE AND is_active = TRUE");
        builder.HasData(PricingSeed.DepositRules);
    }
}
