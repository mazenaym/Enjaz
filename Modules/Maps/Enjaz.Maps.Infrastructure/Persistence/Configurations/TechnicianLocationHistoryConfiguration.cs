using Enjaz.Maps.Domain.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Maps.Infrastructure.Persistence.Configurations;

public sealed class TechnicianLocationHistoryConfiguration : IEntityTypeConfiguration<TechnicianLocationHistory>
{
    public void Configure(EntityTypeBuilder<TechnicianLocationHistory> builder)
    {
        builder.ToTable("technician_location_history");

        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).HasColumnName("id");
        builder.Property(history => history.TechnicianId).HasColumnName("technician_id");
        builder.Property(history => history.UserId).HasColumnName("user_id");
        builder.Property(history => history.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
        builder.Property(history => history.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
        builder.Property(history => history.Location).HasColumnName("location").HasColumnType("geometry(Point,4326)").IsRequired();
        builder.Property(history => history.AccuracyMeters).HasColumnName("accuracy_meters").HasPrecision(8, 2);
        builder.Property(history => history.Heading).HasColumnName("heading").HasPrecision(6, 2);
        builder.Property(history => history.SpeedMetersPerSecond).HasColumnName("speed_meters_per_second").HasPrecision(8, 2);
        builder.Property(history => history.Source).HasColumnName("source").HasMaxLength(50);
        builder.Property(history => history.CreatedAtUtc).HasColumnName("created_at_utc");

        builder.HasIndex(history => history.TechnicianId);
        builder.HasIndex(history => history.CreatedAtUtc);
        builder.HasIndex(history => history.Location).HasMethod("GIST");
    }
}
