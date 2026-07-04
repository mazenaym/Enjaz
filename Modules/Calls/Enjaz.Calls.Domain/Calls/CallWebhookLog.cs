namespace Enjaz.Calls.Domain.Calls;

public sealed class CallWebhookLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Provider { get; set; } = CallProviders.Callera;
    public string? ProviderCallId { get; set; }
    public string? EventType { get; set; }
    public string RawPayloadJson { get; set; } = "{}";
    public string? HeadersJson { get; set; }
    public bool IsProcessed { get; set; }
    public string? ProcessingError { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
}
