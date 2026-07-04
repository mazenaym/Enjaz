using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class LedgerTransactionConfiguration : IEntityTypeConfiguration<LedgerTransaction>
{
    public void Configure(EntityTypeBuilder<LedgerTransaction> builder)
    {
        builder.ToTable("ledger_transactions");
        builder.HasKey(transaction => transaction.Id);
        builder.Property(transaction => transaction.Id).HasColumnName("id");
        builder.Property(transaction => transaction.TransactionNumber).HasColumnName("transaction_number").HasMaxLength(40).IsRequired();
        builder.Property(transaction => transaction.SourceModule).HasColumnName("source_module").HasMaxLength(40).IsRequired();
        builder.Property(transaction => transaction.SourceEntityId).HasColumnName("source_entity_id");
        builder.Property(transaction => transaction.TransactionType).HasColumnName("transaction_type").HasMaxLength(60).IsRequired();
        builder.Property(transaction => transaction.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP").IsRequired();
        builder.Property(transaction => transaction.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2);
        builder.Property(transaction => transaction.IdempotencyKey).HasColumnName("idempotency_key").HasMaxLength(160).IsRequired();
        builder.Property(transaction => transaction.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(transaction => transaction.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(transaction => transaction.TransactionNumber).IsUnique();
        builder.HasIndex(transaction => transaction.IdempotencyKey).IsUnique();
        builder.HasIndex(transaction => new { transaction.SourceModule, transaction.SourceEntityId });
    }
}
