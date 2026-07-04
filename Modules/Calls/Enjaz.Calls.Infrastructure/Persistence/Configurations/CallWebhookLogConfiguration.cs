using Enjaz.Calls.Domain.Calls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Calls.Infrastructure.Persistence.Configurations;

public sealed class CallWebhookLogConfiguration : IEntityTypeConfiguration<CallWebhookLog>
{
    public void Configure(EntityTypeBuilder<CallWebhookLog> builder)
    {
        builder.ToTable("call_webhook_logs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Id).HasColumnName("id");
        builder.Property(log => log.Provider).HasColumnName("provider").HasMaxLength(50).IsRequired();
        builder.Property(log => log.ProviderCallId).HasColumnName("provider_call_id").HasMaxLength(200);
        builder.Property(log => log.EventType).HasColumnName("event_type").HasMaxLength(100);
        builder.Property(log => log.RawPayloadJson).HasColumnName("raw_payload_json").IsRequired();
        builder.Property(log => log.HeadersJson).HasColumnName("headers_json");
        builder.Property(log => log.IsProcessed).HasColumnName("is_processed").HasDefaultValue(false);
        builder.Property(log => log.ProcessingError).HasColumnName("processing_error").HasMaxLength(1000);
        builder.Property(log => log.ReceivedAtUtc).HasColumnName("received_at_utc");
        builder.Property(log => log.ProcessedAtUtc).HasColumnName("processed_at_utc");
        builder.HasIndex(log => log.ProviderCallId);
        builder.HasIndex(log => log.ReceivedAtUtc);
    }
}
