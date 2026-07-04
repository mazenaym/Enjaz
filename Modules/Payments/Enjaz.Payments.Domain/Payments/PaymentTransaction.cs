namespace Enjaz.Payments.Domain.Payments;

public sealed class PaymentTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaymentId { get; set; }
    public string Provider { get; set; } = PaymentProviders.Fake;
    public string? ProviderTransactionId { get; set; }
    public string? ProviderOrderId { get; set; }
    public string TransactionType { get; set; } = PaymentTransactionTypes.CheckoutCreated;
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RawPayloadJson { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
