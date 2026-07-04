namespace Enjaz.Wallets.Domain.Wallets;

public sealed class TechnicianEarning
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid PaymentId { get; set; }
    public Guid? TechnicianId { get; set; }
    public Guid? TechnicianUserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Status { get; set; } = TechnicianEarningStatuses.Pending;
    public DateTime? AvailableAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
