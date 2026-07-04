namespace Enjaz.Wallets.Domain.Wallets;

public sealed class PayoutBatchItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PayoutBatchId { get; set; }
    public Guid TechnicianId { get; set; }
    public Guid TechnicianUserId { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Status { get; set; } = PayoutBatchItemStatuses.Pending;
    public string? FailureReason { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
