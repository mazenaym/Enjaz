using Enjaz.Maps.Domain.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Maps.Infrastructure.Persistence.Configurations;

public sealed class TechnicianLocationSnapshotConfiguration : IEntityTypeConfiguration<TechnicianLocationSnapshot>
{
    public void Configure(EntityTypeBuilder<TechnicianLocationSnapshot> builder)
    {
        builder.ToTable("technician_location_snapshots");

        builder.HasKey(snapshot => snapshot.Id);
        builder.Property(snapshot => snapshot.Id).HasColumnName("id");
        builder.Property(snapshot => snapshot.TechnicianId).HasColumnName("technician_id");
        builder.Property(snapshot => snapshot.UserId).HasColumnName("user_id");
        builder.Property(snapshot => snapshot.Latitude).HasColumnName("latitude").HasPrecision(9, 6);
        builder.Property(snapshot => snapshot.Longitude).HasColumnName("longitude").HasPrecision(9, 6);
        builder.Property(snapshot => snapshot.Location).HasColumnName("location").HasColumnType("geometry(Point,4326)").IsRequired();
        builder.Property(snapshot => snapshot.AccuracyMeters).HasColumnName("accuracy_meters").HasPrecision(8, 2);
        builder.Property(snapshot => snapshot.Heading).HasColumnName("heading").HasPrecision(6, 2);
        builder.Property(snapshot => snapshot.SpeedMetersPerSecond).HasColumnName("speed_meters_per_second").HasPrecision(8, 2);
        builder.Property(snapshot => snapshot.Source).HasColumnName("source").HasMaxLength(50);
        builder.Property(snapshot => snapshot.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.HasIndex(snapshot => snapshot.TechnicianId).IsUnique();
        builder.HasIndex(snapshot => snapshot.Location).HasMethod("GIST");
    }
}
