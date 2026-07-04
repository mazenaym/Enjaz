using Enjaz.Pricing.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Pricing.Infrastructure.Persistence.Configurations;

public sealed class CommissionSettingConfiguration : IEntityTypeConfiguration<CommissionSetting>
{
    public void Configure(EntityTypeBuilder<CommissionSetting> builder)
    {
        builder.ToTable("commission_settings");
        builder.HasKey(setting => setting.Id);
        builder.Property(setting => setting.Id).HasColumnName("id");
        builder.Property(setting => setting.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(setting => setting.CommissionRate).HasColumnName("commission_rate").HasPrecision(5, 4);
        builder.Property(setting => setting.IsDefault).HasColumnName("is_default");
        builder.Property(setting => setting.IsActive).HasColumnName("is_active");
        builder.Property(setting => setting.EffectiveFromUtc).HasColumnName("effective_from_utc");
        builder.Property(setting => setting.EffectiveToUtc).HasColumnName("effective_to_utc");
        builder.Property(setting => setting.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(setting => setting.IsDefault)
            .IsUnique()
            .HasFilter("is_default = TRUE AND is_active = TRUE");
        builder.HasData(PricingSeed.CommissionSettings);
    }
}
