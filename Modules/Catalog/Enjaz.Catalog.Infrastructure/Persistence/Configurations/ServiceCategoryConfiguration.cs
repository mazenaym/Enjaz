using Enjaz.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("service_categories");

        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).HasColumnName("id");
        builder.Property(category => category.NameAr).HasColumnName("name_ar").HasMaxLength(200).IsRequired();
        builder.Property(category => category.NameEn).HasColumnName("name_en").HasMaxLength(200);
        builder.Property(category => category.Slug).HasColumnName("slug").HasMaxLength(200).IsRequired();
        builder.Property(category => category.DescriptionAr).HasColumnName("description_ar").HasMaxLength(1000);
        builder.Property(category => category.DescriptionEn).HasColumnName("description_en").HasMaxLength(1000);
        builder.Property(category => category.IconUrl).HasColumnName("icon_url").HasMaxLength(1000);
        builder.Property(category => category.DisplayOrder).HasColumnName("display_order");
        builder.Property(category => category.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(category => category.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(category => category.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(category => category.Slug).IsUnique();
        builder.HasData(CatalogSeed.Categories);
    }
}
