using Enjaz.Jobs.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Jobs.Infrastructure.Persistence.Configurations;

public sealed class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");
        builder.HasKey(job => job.Id);
        builder.Property(job => job.Id).HasColumnName("id");
        builder.Property(job => job.JobNumber).HasColumnName("job_number").HasMaxLength(32).IsRequired();
        builder.HasIndex(job => job.JobNumber).IsUnique();
        builder.Property(job => job.CustomerUserId).HasColumnName("customer_user_id");
        builder.Property(job => job.CustomerProfileId).HasColumnName("customer_profile_id");
        builder.Property(job => job.CustomerAddressId).HasColumnName("customer_address_id");
        builder.Property(job => job.ServiceCategoryId).HasColumnName("service_category_id");
        builder.Property(job => job.ServiceId).HasColumnName("service_id");
        builder.Property(job => job.ServiceTierId).HasColumnName("service_tier_id");
        builder.Property(job => job.AiClassificationId).HasColumnName("ai_classification_id");
        builder.Property(job => job.PriceSnapshotId).HasColumnName("price_snapshot_id");
        builder.Property(job => job.ServiceZoneId).HasColumnName("service_zone_id");
        builder.Property(job => job.AssignedTechnicianId).HasColumnName("assigned_technician_id");
        builder.Property(job => job.AssignedTechnicianUserId).HasColumnName("assigned_technician_user_id");
        builder.Property(job => job.Title).HasColumnName("title").HasMaxLength(200);
        builder.Property(job => job.Description).HasColumnName("description").HasMaxLength(2000).IsRequired();
        builder.Property(job => job.Status).HasColumnName("status").HasMaxLength(64).IsRequired();
        builder.Property(job => job.ScheduledAtUtc).HasColumnName("scheduled_at_utc");
        builder.Property(job => job.PreferredTimeWindowStartUtc).HasColumnName("preferred_time_window_start_utc");
        builder.Property(job => job.PreferredTimeWindowEndUtc).HasColumnName("preferred_time_window_end_utc");
        builder.Property(job => job.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP").IsRequired();
        builder.Property(job => job.EstimatedTotalAmount).HasColumnName("estimated_total_amount").HasPrecision(18, 2);
        builder.Property(job => job.EstimatedDepositAmount).HasColumnName("estimated_deposit_amount").HasPrecision(18, 2);
        builder.Property(job => job.RequiresInspection).HasColumnName("requires_inspection").HasDefaultValue(false);
        builder.Property(job => job.CancellationReason).HasColumnName("cancellation_reason").HasMaxLength(500);
        builder.Property(job => job.CancelledByUserId).HasColumnName("cancelled_by_user_id");
        builder.Property(job => job.CancelledAtUtc).HasColumnName("cancelled_at_utc");
        builder.Property(job => job.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(job => job.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(job => job.CustomerUserId);
        builder.HasIndex(job => job.CustomerAddressId);
        builder.HasIndex(job => job.ServiceId);
        builder.HasIndex(job => job.Status);
        builder.HasIndex(job => job.AssignedTechnicianId);
        builder.HasIndex(job => job.CreatedAtUtc);
    }
}
