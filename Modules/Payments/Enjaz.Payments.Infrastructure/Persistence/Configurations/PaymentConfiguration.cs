using Enjaz.Payments.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Payments.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Id).HasColumnName("id");
        builder.Property(payment => payment.JobId).HasColumnName("job_id");
        builder.Property(payment => payment.JobNumber).HasColumnName("job_number").HasMaxLength(32).IsRequired();
        builder.Property(payment => payment.CustomerUserId).HasColumnName("customer_user_id");
        builder.Property(payment => payment.PriceSnapshotId).HasColumnName("price_snapshot_id");
        builder.Property(payment => payment.Amount).HasColumnName("amount").HasPrecision(18, 2);
        builder.Property(payment => payment.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP").IsRequired();
        builder.Property(payment => payment.Provider).HasColumnName("provider").HasMaxLength(32).IsRequired();
        builder.Property(payment => payment.Status).HasColumnName("status").HasMaxLength(32).IsRequired();
        builder.Property(payment => payment.CheckoutUrl).HasColumnName("checkout_url").HasMaxLength(1000);
        builder.Property(payment => payment.ProviderOrderId).HasColumnName("provider_order_id").HasMaxLength(200);
        builder.Property(payment => payment.ProviderPaymentKey).HasColumnName("provider_payment_key").HasMaxLength(500);
        builder.Property(payment => payment.ProviderTransactionId).HasColumnName("provider_transaction_id").HasMaxLength(200);
        builder.Property(payment => payment.FailureReason).HasColumnName("failure_reason").HasMaxLength(500);
        builder.Property(payment => payment.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(payment => payment.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(payment => payment.PaidAtUtc).HasColumnName("paid_at_utc");
        builder.Property(payment => payment.FailedAtUtc).HasColumnName("failed_at_utc");
        builder.HasIndex(payment => payment.JobId);
        builder.HasIndex(payment => payment.CustomerUserId);
        builder.HasIndex(payment => payment.Status);
        builder.HasIndex(payment => payment.ProviderOrderId);
        builder.HasIndex(payment => payment.ProviderTransactionId).IsUnique().HasFilter("provider_transaction_id IS NOT NULL");
    }
}
