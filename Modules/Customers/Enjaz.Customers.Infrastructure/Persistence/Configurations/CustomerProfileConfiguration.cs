using Enjaz.Customers.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Customers.Infrastructure.Persistence.Configurations;

public sealed class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("customer_profiles");

        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id).HasColumnName("id");
        builder.Property(profile => profile.UserId).HasColumnName("user_id");
        builder.Property(profile => profile.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();
        builder.Property(profile => profile.PhoneNumber).HasColumnName("phone_number").HasMaxLength(32).IsRequired();
        builder.Property(profile => profile.ProfileImageUrl).HasColumnName("profile_image_url").HasMaxLength(1000);
        builder.Property(profile => profile.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(profile => profile.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(profile => profile.UserId).IsUnique();
    }
}
