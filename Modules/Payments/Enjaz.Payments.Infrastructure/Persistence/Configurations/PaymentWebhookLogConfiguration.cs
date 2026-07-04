using Enjaz.Payments.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Payments.Infrastructure.Persistence.Configurations;

public sealed class PaymentWebhookLogConfiguration : IEntityTypeConfiguration<PaymentWebhookLog>
{
    public void Configure(EntityTypeBuilder<PaymentWebhookLog> builder)
    {
        builder.ToTable("payment_webhook_logs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Id).HasColumnName("id");
        builder.Property(log => log.Provider).HasColumnName("provider").HasMaxLength(32).IsRequired();
        builder.Property(log => log.EventType).HasColumnName("event_type").HasMaxLength(64);
        builder.Property(log => log.ProviderTransactionId).HasColumnName("provider_transaction_id").HasMaxLength(200);
        builder.Property(log => log.ProviderOrderId).HasColumnName("provider_order_id").HasMaxLength(200);
        builder.Property(log => log.RawPayloadJson).HasColumnName("raw_payload_json").HasColumnType("jsonb").IsRequired();
        builder.Property(log => log.HeadersJson).HasColumnName("headers_json").HasColumnType("jsonb");
        builder.Property(log => log.Signature).HasColumnName("signature").HasMaxLength(500);
        builder.Property(log => log.IsProcessed).HasColumnName("is_processed").HasDefaultValue(false);
        builder.Property(log => log.ProcessingError).HasColumnName("processing_error").HasMaxLength(1000);
        builder.Property(log => log.ReceivedAtUtc).HasColumnName("received_at_utc");
        builder.Property(log => log.ProcessedAtUtc).HasColumnName("processed_at_utc");
        builder.HasIndex(log => log.ProviderTransactionId);
        builder.HasIndex(log => log.ReceivedAtUtc);
    }
}
