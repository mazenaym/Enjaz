namespace Enjaz.Payments.Domain.Payments;

public sealed class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public string JobNumber { get; set; } = string.Empty;
    public Guid CustomerUserId { get; set; }
    public Guid PriceSnapshotId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Provider { get; set; } = PaymentProviders.Fake;
    public string Status { get; set; } = PaymentStatuses.Created;
    public string? CheckoutUrl { get; set; }
    public string? ProviderOrderId { get; set; }
    public string? ProviderPaymentKey { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public DateTime? FailedAtUtc { get; set; }
}
