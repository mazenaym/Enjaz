using Enjaz.Catalog.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        builder.HasKey(service => service.Id);
        builder.Property(service => service.Id).HasColumnName("id");
        builder.Property(service => service.CategoryId).HasColumnName("category_id");
        builder.Property(service => service.NameAr).HasColumnName("name_ar").HasMaxLength(200).IsRequired();
        builder.Property(service => service.NameEn).HasColumnName("name_en").HasMaxLength(200);
        builder.Property(service => service.Slug).HasColumnName("slug").HasMaxLength(200).IsRequired();
        builder.Property(service => service.DescriptionAr).HasColumnName("description_ar").HasMaxLength(1000);
        builder.Property(service => service.DescriptionEn).HasColumnName("description_en").HasMaxLength(1000);
        builder.Property(service => service.IconUrl).HasColumnName("icon_url").HasMaxLength(1000);
        builder.Property(service => service.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(service => service.DisplayOrder).HasColumnName("display_order");
        builder.Property(service => service.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(service => service.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasOne(service => service.Category)
            .WithMany(category => category.Services)
            .HasForeignKey(service => service.CategoryId);

        builder.HasIndex(service => service.Slug).IsUnique();
        builder.HasIndex(service => service.CategoryId);
        builder.HasData(CatalogSeed.Services);
    }
}
