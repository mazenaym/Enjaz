namespace Enjaz.Wallets.Domain.Wallets;

public sealed class Wallet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerType { get; set; } = string.Empty;
    public Guid? OwnerUserId { get; set; }
    public Guid? TechnicianId { get; set; }
    public string Currency { get; set; } = "EGP";
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalCredited { get; set; }
    public decimal TotalDebited { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
