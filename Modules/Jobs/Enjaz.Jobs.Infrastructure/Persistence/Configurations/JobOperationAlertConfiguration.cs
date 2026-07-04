using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobOperationAlertConfiguration : IEntityTypeConfiguration<JobOperationAlert>
{
    public void Configure(EntityTypeBuilder<JobOperationAlert> builder)
    {
        builder.ToTable("job_operation_alerts");
        builder.HasKey(alert => alert.Id);
        builder.Property(alert => alert.Id).HasColumnName("id");
        builder.Property(alert => alert.JobId).HasColumnName("job_id");
        builder.Property(alert => alert.AlertType).HasColumnName("alert_type").HasMaxLength(80).IsRequired();
        builder.Property(alert => alert.IsResolved).HasColumnName("is_resolved").HasDefaultValue(false);
        builder.Property(alert => alert.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(alert => alert.ResolvedAtUtc).HasColumnName("resolved_at_utc");
        builder.HasIndex(alert => new { alert.JobId, alert.AlertType, alert.IsResolved });
    }
}
