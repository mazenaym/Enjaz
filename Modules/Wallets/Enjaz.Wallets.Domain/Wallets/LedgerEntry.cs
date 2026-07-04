namespace Enjaz.Wallets.Domain.Wallets;

public sealed class LedgerEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LedgerTransactionId { get; set; }
    public Guid WalletId { get; set; }
    public string EntryDirection { get; set; } = string.Empty;
    public string BalanceType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
