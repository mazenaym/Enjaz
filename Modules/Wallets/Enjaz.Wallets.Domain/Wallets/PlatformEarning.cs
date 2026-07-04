namespace Enjaz.Wallets.Domain.Wallets;

public sealed class PlatformEarning
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaymentId { get; set; }
    public Guid JobId { get; set; }
    public Guid PriceSnapshotId { get; set; }
    public Guid CustomerUserId { get; set; }
    public Guid? TechnicianId { get; set; }
    public Guid? TechnicianUserId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Status { get; set; } = PlatformEarningStatuses.Recorded;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
