using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobAssignmentConfiguration : IEntityTypeConfiguration<JobAssignment>
{
    public void Configure(EntityTypeBuilder<JobAssignment> builder)
    {
        builder.ToTable("job_assignments");
        builder.HasKey(assignment => assignment.Id);
        builder.Property(assignment => assignment.Id).HasColumnName("id");
        builder.Property(assignment => assignment.JobId).HasColumnName("job_id").IsRequired();
        builder.Property(assignment => assignment.TechnicianId).HasColumnName("technician_id").IsRequired();
        builder.Property(assignment => assignment.TechnicianUserId).HasColumnName("technician_user_id").IsRequired();
        builder.Property(assignment => assignment.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(assignment => assignment.OfferedAtUtc).HasColumnName("offered_at_utc");
        builder.Property(assignment => assignment.RespondedAtUtc).HasColumnName("responded_at_utc");
        builder.Property(assignment => assignment.ExpiresAtUtc).HasColumnName("expires_at_utc");
        builder.Property(assignment => assignment.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(500);
        builder.HasIndex(assignment => assignment.JobId);
        builder.HasIndex(assignment => assignment.TechnicianId);
        builder.HasIndex(assignment => assignment.Status);
    }
}
