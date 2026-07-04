namespace Enjaz.Payments.Application.Payments;

public sealed record CreateCheckoutRequest(Guid JobId);
public sealed record FakePaymentFailRequest(string? Reason);

public sealed record PaymentCheckoutResponse(Guid PaymentId, Guid JobId, decimal Amount, string Currency, string Status, string Provider, string? CheckoutUrl);
public sealed record PaymentSummaryResponse(Guid Id, Guid JobId, string JobNumber, decimal Amount, string Currency, string Provider, string Status, DateTime CreatedAtUtc, DateTime? PaidAtUtc, DateTime? FailedAtUtc);
public sealed record PaymentDetailsResponse(Guid Id, Guid JobId, string JobNumber, Guid CustomerUserId, Guid PriceSnapshotId, decimal Amount, string Currency, string Provider, string Status, string? CheckoutUrl, string? ProviderOrderId, string? ProviderTransactionId, string? FailureReason, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, DateTime? PaidAtUtc, DateTime? FailedAtUtc, IReadOnlyCollection<PaymentTransactionResponse> Transactions);
public sealed record PaymentTransactionResponse(Guid Id, string Provider, string? ProviderTransactionId, string? ProviderOrderId, string TransactionType, decimal? Amount, string? Currency, string Status, DateTime CreatedAtUtc);
public sealed record PaymentWebhookLogResponse(Guid Id, string Provider, string? EventType, string? ProviderTransactionId, string? ProviderOrderId, bool IsProcessed, string? ProcessingError, DateTime ReceivedAtUtc, DateTime? ProcessedAtUtc);

public sealed record CreateCheckoutSessionProviderRequest(Guid PaymentId, Guid JobId, string JobNumber, Guid CustomerUserId, decimal Amount, string Currency, string? CustomerPhone, string? CustomerEmail, string? ReturnUrl, string? CallbackUrl);
public sealed record CreateCheckoutSessionProviderResponse(string Provider, string ProviderOrderId, string ProviderPaymentKey, string CheckoutUrl, string RawResponseJson);
public sealed record PaymentWebhookVerificationRequest(string RawBody, IReadOnlyDictionary<string, string> Headers, string? Signature);
public sealed record PaymentWebhookParseRequest(string RawBody);
public sealed record ParsedPaymentWebhook(string EventType, string? ProviderTransactionId, string? ProviderOrderId, string Status, decimal? Amount, string? Currency, string RawPayloadJson);
