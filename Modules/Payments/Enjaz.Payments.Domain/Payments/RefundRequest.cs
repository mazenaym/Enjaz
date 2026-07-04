namespace Enjaz.Payments.Domain.Payments;

public sealed class RefundRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaymentId { get; set; }
    public Guid JobId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = RefundRequestStatuses.Requested;
    public Guid RequestedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
