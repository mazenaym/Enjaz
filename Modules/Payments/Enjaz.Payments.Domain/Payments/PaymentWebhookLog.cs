namespace Enjaz.Payments.Domain.Payments;

public sealed class PaymentWebhookLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Provider { get; set; } = PaymentProviders.Paymob;
    public string? EventType { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? ProviderOrderId { get; set; }
    public string RawPayloadJson { get; set; } = string.Empty;
    public string? HeadersJson { get; set; }
    public string? Signature { get; set; }
    public bool IsProcessed { get; set; }
    public string? ProcessingError { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
}
