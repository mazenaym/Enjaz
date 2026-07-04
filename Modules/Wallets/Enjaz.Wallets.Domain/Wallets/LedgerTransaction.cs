namespace Enjaz.Wallets.Domain.Wallets;

public sealed class LedgerTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TransactionNumber { get; set; } = string.Empty;
    public string SourceModule { get; set; } = string.Empty;
    public Guid SourceEntityId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Currency { get; set; } = "EGP";
    public decimal TotalAmount { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
