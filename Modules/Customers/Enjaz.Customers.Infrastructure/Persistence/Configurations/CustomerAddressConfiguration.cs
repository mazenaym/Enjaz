using Enjaz.Customers.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Customers.Infrastructure.Persistence.Configurations;

public sealed class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("customer_addresses");

        builder.HasKey(address => address.Id);

        builder.Property(address => address.Id).HasColumnName("id");
        builder.Property(address => address.CustomerId).HasColumnName("customer_id");
        builder.Property(address => address.Label).HasColumnName("label").HasMaxLength(100);
        builder.Property(address => address.City).HasColumnName("city").HasMaxLength(100).IsRequired();
        builder.Property(address => address.Area).HasColumnName("area").HasMaxLength(100).IsRequired();
        builder.Property(address => address.Street).HasColumnName("street").HasMaxLength(200).IsRequired();
        builder.Property(address => address.BuildingNumber).HasColumnName("building_number").HasMaxLength(50);
        builder.Property(address => address.Floor).HasColumnName("floor").HasMaxLength(50);
        builder.Property(address => address.Apartment).HasColumnName("apartment").HasMaxLength(50);
        builder.Property(address => address.Landmark).HasColumnName("landmark").HasMaxLength(300);
        builder.Property(address => address.FormattedAddress).HasColumnName("formatted_address").HasMaxLength(1000);
        builder.Property(address => address.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
        builder.Property(address => address.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
        builder.Property(address => address.IsDefault).HasColumnName("is_default");
        builder.Property(address => address.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(address => address.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder
            .HasOne(address => address.Customer)
            .WithMany(profile => profile.Addresses)
            .HasForeignKey(address => address.CustomerId);

        builder.HasIndex(address => new { address.CustomerId, address.IsDefault });
    }
}
