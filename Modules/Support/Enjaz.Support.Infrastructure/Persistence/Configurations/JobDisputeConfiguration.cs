using Enjaz.Support.Domain.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Support.Infrastructure.Persistence.Configurations;

public sealed class JobDisputeConfiguration : IEntityTypeConfiguration<JobDispute>
{
    public void Configure(EntityTypeBuilder<JobDispute> builder)
    {
        builder.ToTable("job_disputes");
        builder.HasKey(dispute => dispute.Id);
        builder.Property(dispute => dispute.Id).HasColumnName("id");
        builder.Property(dispute => dispute.JobId).HasColumnName("job_id");
        builder.Property(dispute => dispute.OpenedByUserId).HasColumnName("opened_by_user_id");
        builder.Property(dispute => dispute.Reason).HasColumnName("reason").HasMaxLength(2000).IsRequired();
        builder.Property(dispute => dispute.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(dispute => dispute.Resolution).HasColumnName("resolution").HasMaxLength(2000);
        builder.Property(dispute => dispute.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(dispute => dispute.ResolvedAtUtc).HasColumnName("resolved_at_utc");
        builder.HasIndex(dispute => dispute.JobId);
        builder.HasIndex(dispute => dispute.Status);
    }
}
