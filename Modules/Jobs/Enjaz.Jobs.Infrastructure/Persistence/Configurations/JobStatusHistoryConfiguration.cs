using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobStatusHistoryConfiguration : IEntityTypeConfiguration<JobStatusHistory>
{
    public void Configure(EntityTypeBuilder<JobStatusHistory> builder)
    {
        builder.ToTable("job_status_history");
        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).HasColumnName("id");
        builder.Property(history => history.JobId).HasColumnName("job_id").IsRequired();
        builder.Property(history => history.FromStatus).HasColumnName("from_status").HasMaxLength(64);
        builder.Property(history => history.ToStatus).HasColumnName("to_status").HasMaxLength(64).IsRequired();
        builder.Property(history => history.ChangedByUserId).HasColumnName("changed_by_user_id");
        builder.Property(history => history.Reason).HasColumnName("reason").HasMaxLength(500);
        builder.Property(history => history.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(history => history.JobId);
    }
}
