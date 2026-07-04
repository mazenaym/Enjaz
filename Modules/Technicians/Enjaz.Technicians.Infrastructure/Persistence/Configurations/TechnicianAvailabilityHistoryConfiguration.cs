using Enjaz.Technicians.Domain.Technicians;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Technicians.Infrastructure.Persistence.Configurations;

public sealed class TechnicianAvailabilityHistoryConfiguration : IEntityTypeConfiguration<TechnicianAvailabilityHistory>
{
    public void Configure(EntityTypeBuilder<TechnicianAvailabilityHistory> builder)
    {
        builder.ToTable("technician_availability_history");

        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).HasColumnName("id");
        builder.Property(history => history.TechnicianId).HasColumnName("technician_id");
        builder.Property(history => history.FromStatus).HasColumnName("from_status").HasMaxLength(50);
        builder.Property(history => history.ToStatus).HasColumnName("to_status").HasMaxLength(50).IsRequired();
        builder.Property(history => history.ChangedAtUtc).HasColumnName("changed_at_utc");
        builder.Property(history => history.ChangedByUserId).HasColumnName("changed_by_user_id");

        builder.HasOne(history => history.Technician)
            .WithMany(profile => profile.AvailabilityHistory)
            .HasForeignKey(history => history.TechnicianId);

        builder.HasIndex(history => history.TechnicianId);
    }
}
