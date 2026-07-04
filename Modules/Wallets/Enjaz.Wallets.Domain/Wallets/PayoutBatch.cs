namespace Enjaz.Wallets.Domain.Wallets;

public sealed class PayoutBatch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string BatchNumber { get; set; } = string.Empty;
    public string Status { get; set; } = PayoutBatchStatuses.Draft;
    public string Currency { get; set; } = "EGP";
    public decimal TotalAmount { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
