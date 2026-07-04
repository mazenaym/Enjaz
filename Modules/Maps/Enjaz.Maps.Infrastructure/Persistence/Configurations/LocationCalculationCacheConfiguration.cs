using Enjaz.Maps.Domain.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Maps.Infrastructure.Persistence.Configurations;

public sealed class LocationCalculationCacheConfiguration : IEntityTypeConfiguration<LocationCalculationCache>
{
    public void Configure(EntityTypeBuilder<LocationCalculationCache> builder)
    {
        builder.ToTable("location_calculation_cache");

        builder.HasKey(cache => cache.Id);
        builder.Property(cache => cache.Id).HasColumnName("id");
        builder.Property(cache => cache.OriginLatitude).HasColumnName("origin_latitude").HasPrecision(9, 6);
        builder.Property(cache => cache.OriginLongitude).HasColumnName("origin_longitude").HasPrecision(9, 6);
        builder.Property(cache => cache.DestinationLatitude).HasColumnName("destination_latitude").HasPrecision(9, 6);
        builder.Property(cache => cache.DestinationLongitude).HasColumnName("destination_longitude").HasPrecision(9, 6);
        builder.Property(cache => cache.DistanceMeters).HasColumnName("distance_meters").HasPrecision(12, 2);
        builder.Property(cache => cache.DurationSeconds).HasColumnName("duration_seconds");
        builder.Property(cache => cache.Provider).HasColumnName("provider").HasMaxLength(100);
        builder.Property(cache => cache.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(cache => cache.ExpiresAtUtc).HasColumnName("expires_at_utc");

        builder.HasIndex(cache => cache.ExpiresAtUtc);
    }
}
