using Enjaz.Calls.Domain.Calls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Calls.Infrastructure.Persistence.Configurations;

public sealed class CallSessionConfiguration : IEntityTypeConfiguration<CallSession>
{
    public void Configure(EntityTypeBuilder<CallSession> builder)
    {
        builder.ToTable("call_sessions");
        builder.HasKey(call => call.Id);
        builder.Property(call => call.Id).HasColumnName("id");
        builder.Property(call => call.JobId).HasColumnName("job_id");
        builder.Property(call => call.CustomerUserId).HasColumnName("customer_user_id");
        builder.Property(call => call.TechnicianUserId).HasColumnName("technician_user_id");
        builder.Property(call => call.TechnicianId).HasColumnName("technician_id");
        builder.Property(call => call.InitiatedByUserId).HasColumnName("initiated_by_user_id");
        builder.Property(call => call.Provider).HasColumnName("provider").HasMaxLength(50).IsRequired();
        builder.Property(call => call.ProviderCallId).HasColumnName("provider_call_id").HasMaxLength(200);
        builder.Property(call => call.MaskedNumber).HasColumnName("masked_number").HasMaxLength(40);
        builder.Property(call => call.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(call => call.StartedAtUtc).HasColumnName("started_at_utc");
        builder.Property(call => call.EndedAtUtc).HasColumnName("ended_at_utc");
        builder.Property(call => call.DurationSeconds).HasColumnName("duration_seconds");
        builder.Property(call => call.RecordingUrl).HasColumnName("recording_url").HasMaxLength(1000);
        builder.Property(call => call.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(call => call.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.HasIndex(call => call.JobId);
        builder.HasIndex(call => call.ProviderCallId);
        builder.HasIndex(call => call.Status);
    }
}
