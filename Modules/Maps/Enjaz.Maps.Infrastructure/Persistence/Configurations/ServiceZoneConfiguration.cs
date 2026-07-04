using Enjaz.Maps.Domain.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Enjaz.Maps.Infrastructure.Persistence.Configurations;

public sealed class ServiceZoneConfiguration : IEntityTypeConfiguration<ServiceZone>
{
    public void Configure(EntityTypeBuilder<ServiceZone> builder)
    {
        builder.ToTable("service_zones");

        builder.HasKey(zone => zone.Id);
        builder.Property(zone => zone.Id).HasColumnName("id");
        builder.Property(zone => zone.NameAr).HasColumnName("name_ar").HasMaxLength(200).IsRequired();
        builder.Property(zone => zone.NameEn).HasColumnName("name_en").HasMaxLength(200);
        builder.Property(zone => zone.City).HasColumnName("city").HasMaxLength(120).IsRequired();
        builder.Property(zone => zone.Area).HasColumnName("area").HasMaxLength(120);
        builder.Property(zone => zone.Slug).HasColumnName("slug").HasMaxLength(200).IsRequired();
        builder.Property(zone => zone.Polygon).HasColumnName("polygon").HasColumnType("geometry(Polygon,4326)").IsRequired();
        builder.Property(zone => zone.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(zone => zone.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(zone => zone.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(zone => zone.Slug).IsUnique();
        builder.HasIndex(zone => zone.IsActive);
        builder.HasIndex(zone => zone.Polygon).HasMethod("GIST");

        builder.HasData(MapsSeed.ServiceZones);
    }
}

internal static class MapsSeed
{
    private static readonly DateTime SeededAtUtc = new(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly ServiceZone[] ServiceZones =
    [
        new()
        {
            Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            NameAr = "Dev Cairo Zone",
            NameEn = "Dev Cairo Zone",
            City = "Cairo",
            Area = "Nasr City",
            Slug = "dev-cairo-zone",
            Polygon = CreatePolygon(
                (30.0700, 31.3000),
                (30.0800, 31.3500),
                (30.0300, 31.3600),
                (30.0200, 31.3100),
                (30.0700, 31.3000)),
            IsActive = true,
            CreatedAtUtc = SeededAtUtc
        }
    ];

    private static Polygon CreatePolygon(params (double Latitude, double Longitude)[] points)
    {
        var coordinates = points
            .Select(point => new Coordinate(point.Longitude, point.Latitude))
            .ToArray();

        return new Polygon(new LinearRing(coordinates)) { SRID = 4326 };
    }
}
