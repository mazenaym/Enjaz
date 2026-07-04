using Enjaz.Payments.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Payments.Infrastructure.Persistence.Configurations;

public sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("payment_transactions");
        builder.HasKey(transaction => transaction.Id);
        builder.Property(transaction => transaction.Id).HasColumnName("id");
        builder.Property(transaction => transaction.PaymentId).HasColumnName("payment_id");
        builder.Property(transaction => transaction.Provider).HasColumnName("provider").HasMaxLength(32).IsRequired();
        builder.Property(transaction => transaction.ProviderTransactionId).HasColumnName("provider_transaction_id").HasMaxLength(200);
        builder.Property(transaction => transaction.ProviderOrderId).HasColumnName("provider_order_id").HasMaxLength(200);
        builder.Property(transaction => transaction.TransactionType).HasColumnName("transaction_type").HasMaxLength(64).IsRequired();
        builder.Property(transaction => transaction.Amount).HasColumnName("amount").HasPrecision(18, 2);
        builder.Property(transaction => transaction.Currency).HasColumnName("currency").HasMaxLength(3);
        builder.Property(transaction => transaction.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(transaction => transaction.RawPayloadJson).HasColumnName("raw_payload_json").HasColumnType("jsonb");
        builder.Property(transaction => transaction.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(transaction => transaction.PaymentId);
        builder.HasIndex(transaction => transaction.ProviderTransactionId);
    }
}
