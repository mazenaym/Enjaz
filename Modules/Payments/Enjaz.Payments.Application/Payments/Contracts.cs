using Enjaz.Payments.Domain.Payments;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Payments.Application.Payments;

public interface IPaymentsService
{
    Task<Result<PaymentCheckoutResponse>> CreateCheckoutAsync(CreateCheckoutRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaymentDetailsResponse>> GetMyPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PaymentSummaryResponse>>> GetMyPaymentsAsync(CancellationToken cancellationToken = default);
    Task<Result<PaymentDetailsResponse>> SimulateFakeSuccessAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<PaymentDetailsResponse>> SimulateFakeFailureAsync(Guid paymentId, FakePaymentFailRequest request, CancellationToken cancellationToken = default);
    Task<Result> ProcessPaymobWebhookAsync(string rawBody, IReadOnlyDictionary<string, string> headers, string? signature, CancellationToken cancellationToken = default);
}

public interface IAdminPaymentsService
{
    Task<Result<IReadOnlyCollection<PaymentSummaryResponse>>> GetPaymentsAsync(string? status, Guid? jobId, Guid? customerUserId, CancellationToken cancellationToken = default);
    Task<Result<PaymentDetailsResponse>> GetPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PaymentSummaryResponse>>> GetPaymentsForJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<PaymentWebhookLogResponse>>> GetWebhookLogsAsync(CancellationToken cancellationToken = default);
    Task<Result<PaymentWebhookLogResponse>> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IPaymentsRepository
{
    IQueryable<Payment> QueryPayments();
    Task<Payment?> GetPaymentAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetActivePaymentForJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentByProviderOrderIdAsync(string providerOrderId, CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentByProviderTransactionIdAsync(string providerTransactionId, CancellationToken cancellationToken = default);
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PaymentTransaction>> GetTransactionsAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<bool> TransactionExistsAsync(string providerTransactionId, string transactionType, CancellationToken cancellationToken = default);
    Task AddTransactionAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);
    IQueryable<PaymentWebhookLog> QueryWebhookLogs();
    Task<PaymentWebhookLog?> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddWebhookLogAsync(PaymentWebhookLog log, CancellationToken cancellationToken = default);
    Task AddRefundRequestAsync(RefundRequest refundRequest, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IPaymentProvider
{
    string ProviderName { get; }
    Task<Result<CreateCheckoutSessionProviderResponse>> CreateCheckoutSessionAsync(CreateCheckoutSessionProviderRequest request, CancellationToken cancellationToken = default);
    Task<Result> VerifyWebhookAsync(PaymentWebhookVerificationRequest request, CancellationToken cancellationToken = default);
    Task<Result<ParsedPaymentWebhook>> ParseWebhookAsync(PaymentWebhookParseRequest request, CancellationToken cancellationToken = default);
}
