using Enjaz.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ServiceTierConfiguration : IEntityTypeConfiguration<ServiceTier>
{
    public void Configure(EntityTypeBuilder<ServiceTier> builder)
    {
        builder.ToTable("service_tiers");

        builder.HasKey(tier => tier.Id);
        builder.Property(tier => tier.Id).HasColumnName("id");
        builder.Property(tier => tier.ServiceId).HasColumnName("service_id");
        builder.Property(tier => tier.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(tier => tier.DescriptionAr).HasColumnName("description_ar").HasMaxLength(1000);
        builder.Property(tier => tier.DescriptionEn).HasColumnName("description_en").HasMaxLength(1000);
        builder.Property(tier => tier.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(tier => tier.DisplayOrder).HasColumnName("display_order");
        builder.Property(tier => tier.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(tier => tier.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasOne(tier => tier.Service)
            .WithMany(service => service.Tiers)
            .HasForeignKey(tier => tier.ServiceId);

        builder.HasIndex(tier => tier.ServiceId);
        builder.HasData(CatalogSeed.ServiceTiers);
    }
}
