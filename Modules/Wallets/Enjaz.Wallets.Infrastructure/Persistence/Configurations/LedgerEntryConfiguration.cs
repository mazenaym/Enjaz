using Enjaz.Wallets.Domain.Wallets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Wallets.Infrastructure.Persistence.Configurations;

public sealed class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("ledger_entries");
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id).HasColumnName("id");
        builder.Property(entry => entry.LedgerTransactionId).HasColumnName("ledger_transaction_id");
        builder.Property(entry => entry.WalletId).HasColumnName("wallet_id");
        builder.Property(entry => entry.EntryDirection).HasColumnName("entry_direction").HasMaxLength(10).IsRequired();
        builder.Property(entry => entry.BalanceType).HasColumnName("balance_type").HasMaxLength(20).IsRequired();
        builder.Property(entry => entry.Amount).HasColumnName("amount").HasPrecision(18, 2);
        builder.Property(entry => entry.Currency).HasColumnName("currency").HasMaxLength(3).HasDefaultValue("EGP").IsRequired();
        builder.Property(entry => entry.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(entry => entry.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.HasIndex(entry => entry.LedgerTransactionId);
        builder.HasIndex(entry => entry.WalletId);
    }
}
