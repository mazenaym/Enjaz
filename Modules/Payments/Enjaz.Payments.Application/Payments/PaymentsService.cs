using System.Text.Json;
using Enjaz.Jobs.Application.Jobs;
using Enjaz.Jobs.Domain.Jobs;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Enjaz.Payments.Domain.Payments;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Enjaz.Wallets.Application.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Payments.Application.Payments;

public sealed class PaymentsService(
    IPaymentsRepository repository,
    IPaymentProvider paymentProvider,
    ICurrentUserContext currentUserContext,
    IJobPaymentLookupService jobPaymentLookupService,
    IJobPaymentStatusService jobPaymentStatusService,
    IPaymentLedgerService paymentLedgerService,
    IRefundLedgerService refundLedgerService,
    INotificationService notificationService)
    : IPaymentsService, IAdminPaymentsService
{
    public async Task<Result<PaymentCheckoutResponse>> CreateCheckoutAsync(CreateCheckoutRequest request, CancellationToken cancellationToken = default)
    {
        var userId = currentUserContext.UserId;
        var job = await jobPaymentLookupService.GetPayableJobAsync(request.JobId, userId, cancellationToken);
        if (job is null)
        {
            return Result.Failure<PaymentCheckoutResponse>("job_not_payable", "Job is not payable.");
        }

        var existing = await repository.GetActivePaymentForJobAsync(job.JobId, cancellationToken);
        if (existing is not null)
        {
            return Result.Success(MapCheckout(existing));
        }

        var amount = job.EstimatedDepositAmount > 0 ? job.EstimatedDepositAmount : job.EstimatedTotalAmount;
        if (amount <= 0)
        {
            return Result.Failure<PaymentCheckoutResponse>("invalid_payment_amount", "Payment amount must be greater than zero.");
        }

        var now = DateTime.UtcNow;
        var payment = new Payment
        {
            JobId = job.JobId,
            JobNumber = job.JobNumber,
            CustomerUserId = job.CustomerUserId,
            PriceSnapshotId = job.PriceSnapshotId,
            Amount = amount,
            Currency = job.Currency,
            Provider = paymentProvider.ProviderName,
            Status = PaymentStatuses.Created,
            CreatedAtUtc = now
        };

        await repository.AddPaymentAsync(payment, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var providerResult = await paymentProvider.CreateCheckoutSessionAsync(new CreateCheckoutSessionProviderRequest(payment.Id, payment.JobId, payment.JobNumber, payment.CustomerUserId, payment.Amount, payment.Currency, null, null, null, null), cancellationToken);
        if (providerResult.IsFailure)
        {
            payment.Status = PaymentStatuses.Failed;
            payment.FailureReason = providerResult.ErrorMessage;
            payment.FailedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Failure<PaymentCheckoutResponse>(providerResult.ErrorCode!, providerResult.ErrorMessage!);
        }

        var provider = providerResult.Value!;
        payment.Status = PaymentStatuses.Pending;
        payment.CheckoutUrl = provider.CheckoutUrl;
        payment.ProviderOrderId = provider.ProviderOrderId;
        payment.ProviderPaymentKey = provider.ProviderPaymentKey;
        payment.UpdatedAtUtc = DateTime.UtcNow;
        await repository.AddTransactionAsync(new PaymentTransaction { PaymentId = payment.Id, Provider = payment.Provider, ProviderOrderId = payment.ProviderOrderId, TransactionType = PaymentTransactionTypes.CheckoutCreated, Amount = payment.Amount, Currency = payment.Currency, Status = payment.Status, RawPayloadJson = provider.RawResponseJson, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await NotifyPaymentAsync(payment, NotificationTypes.PaymentCheckoutCreated, "Payment checkout created", "Your payment checkout was created.", cancellationToken);
        return Result.Success(MapCheckout(payment));
    }

    public async Task<Result<PaymentDetailsResponse>> GetMyPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await repository.GetPaymentAsync(id, cancellationToken);
        if (payment is null || payment.CustomerUserId != currentUserContext.UserId)
        {
            return Result.Failure<PaymentDetailsResponse>("payment_not_found", "Payment was not found.");
        }

        return Result.Success(await MapDetailsAsync(payment, cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<PaymentSummaryResponse>>> GetMyPaymentsAsync(CancellationToken cancellationToken = default)
    {
        var payments = await repository.QueryPayments().AsNoTracking().Where(payment => payment.CustomerUserId == currentUserContext.UserId).OrderByDescending(payment => payment.CreatedAtUtc).Select(payment => MapSummary(payment)).ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<PaymentSummaryResponse>>(payments);
    }

    public async Task<Result<PaymentDetailsResponse>> SimulateFakeSuccessAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await CompletePaymentAsync(paymentId, $"fake-txn-{paymentId}", PaymentStatuses.Succeeded, null, JsonSerializer.Serialize(new { fake = true, status = "succeeded", paymentId }), cancellationToken);
    }

    public async Task<Result<PaymentDetailsResponse>> SimulateFakeFailureAsync(Guid paymentId, FakePaymentFailRequest request, CancellationToken cancellationToken = default)
    {
        return await CompletePaymentAsync(paymentId, $"fake-txn-{paymentId}", PaymentStatuses.Failed, request.Reason ?? "Fake payment failed.", JsonSerializer.Serialize(new { fake = true, status = "failed", paymentId, request.Reason }), cancellationToken);
    }

    public async Task<Result> ProcessPaymobWebhookAsync(string rawBody, IReadOnlyDictionary<string, string> headers, string? signature, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawBody))
        {
            return Result.Failure("empty_webhook", "Webhook body is required.");
        }

        var verification = await paymentProvider.VerifyWebhookAsync(new PaymentWebhookVerificationRequest(rawBody, headers, signature), cancellationToken);
        var parseResult = await paymentProvider.ParseWebhookAsync(new PaymentWebhookParseRequest(rawBody), cancellationToken);
        var parsed = parseResult.Value;
        var log = new PaymentWebhookLog
        {
            Provider = PaymentProviders.Paymob,
            EventType = parsed?.EventType,
            ProviderTransactionId = parsed?.ProviderTransactionId,
            ProviderOrderId = parsed?.ProviderOrderId,
            RawPayloadJson = rawBody,
            HeadersJson = JsonSerializer.Serialize(headers),
            Signature = signature,
            ReceivedAtUtc = DateTime.UtcNow
        };

        await repository.AddWebhookLogAsync(log, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        if (verification.IsFailure || parseResult.IsFailure || parsed is null)
        {
            log.ProcessingError = verification.IsFailure ? verification.ErrorMessage : parseResult.ErrorMessage;
            log.ProcessedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        if (!string.IsNullOrWhiteSpace(parsed.ProviderTransactionId) && await repository.TransactionExistsAsync(parsed.ProviderTransactionId, parsed.EventType, cancellationToken))
        {
            log.IsProcessed = true;
            log.ProcessedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        var payment = !string.IsNullOrWhiteSpace(parsed.ProviderTransactionId)
            ? await repository.GetPaymentByProviderTransactionIdAsync(parsed.ProviderTransactionId, cancellationToken)
            : null;
        payment ??= !string.IsNullOrWhiteSpace(parsed.ProviderOrderId)
            ? await repository.GetPaymentByProviderOrderIdAsync(parsed.ProviderOrderId, cancellationToken)
            : null;

        if (payment is null)
        {
            log.ProcessingError = "Payment was not found for webhook.";
            log.ProcessedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        if (parsed.Amount.HasValue && parsed.Amount.Value != payment.Amount)
        {
            log.ProcessingError = "Webhook amount mismatch.";
            log.ProcessedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        var status = parsed.Status.Equals("succeeded", StringComparison.OrdinalIgnoreCase) || parsed.Status.Equals("success", StringComparison.OrdinalIgnoreCase)
            ? PaymentStatuses.Succeeded
            : PaymentStatuses.Failed;
        var completion = await CompletePaymentAsync(payment.Id, parsed.ProviderTransactionId ?? $"paymob-{log.Id}", status, status == PaymentStatuses.Failed ? parsed.Status : null, rawBody, cancellationToken);
        log.IsProcessed = true;
        log.ProcessingError = completion.IsFailure ? completion.ErrorMessage : null;
        log.ProcessedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<PaymentSummaryResponse>>> GetPaymentsAsync(string? status, Guid? jobId, Guid? customerUserId, CancellationToken cancellationToken = default)
    {
        var query = repository.QueryPayments().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(payment => payment.Status == status);
        if (jobId.HasValue) query = query.Where(payment => payment.JobId == jobId.Value);
        if (customerUserId.HasValue) query = query.Where(payment => payment.CustomerUserId == customerUserId.Value);
        var payments = await query.OrderByDescending(payment => payment.CreatedAtUtc).Select(payment => MapSummary(payment)).ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<PaymentSummaryResponse>>(payments);
    }

    public async Task<Result<PaymentDetailsResponse>> GetPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await repository.GetPaymentAsync(id, cancellationToken);
        return payment is null ? Result.Failure<PaymentDetailsResponse>("payment_not_found", "Payment was not found.") : Result.Success(await MapDetailsAsync(payment, cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<PaymentSummaryResponse>>> GetPaymentsForJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var payments = await repository.QueryPayments().AsNoTracking().Where(payment => payment.JobId == jobId).OrderByDescending(payment => payment.CreatedAtUtc).Select(payment => MapSummary(payment)).ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<PaymentSummaryResponse>>(payments);
    }

    public async Task<Result<IReadOnlyCollection<PaymentWebhookLogResponse>>> GetWebhookLogsAsync(CancellationToken cancellationToken = default)
    {
        var logs = await repository.QueryWebhookLogs().AsNoTracking().OrderByDescending(log => log.ReceivedAtUtc).Take(200).Select(log => MapWebhookLog(log)).ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<PaymentWebhookLogResponse>>(logs);
    }

    public async Task<Result<PaymentWebhookLogResponse>> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var log = await repository.GetWebhookLogAsync(id, cancellationToken);
        return log is null ? Result.Failure<PaymentWebhookLogResponse>("webhook_log_not_found", "Webhook log was not found.") : Result.Success(MapWebhookLog(log));
    }

    private async Task<Result<PaymentDetailsResponse>> CompletePaymentAsync(Guid paymentId, string providerTransactionId, string status, string? failureReason, string rawPayloadJson, CancellationToken cancellationToken)
    {
        var payment = await repository.GetPaymentAsync(paymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<PaymentDetailsResponse>("payment_not_found", "Payment was not found.");
        }

        var transactionType = status == PaymentStatuses.Succeeded ? PaymentTransactionTypes.PaymentSucceeded : PaymentTransactionTypes.PaymentFailed;
        if (await repository.TransactionExistsAsync(providerTransactionId, transactionType, cancellationToken))
        {
            if (status == PaymentStatuses.Succeeded)
            {
                var ledgerResult = await RecordSuccessfulPaymentLedgerAsync(payment, cancellationToken);
                if (ledgerResult.IsFailure) return Result.Failure<PaymentDetailsResponse>(ledgerResult.ErrorCode!, ledgerResult.ErrorMessage!);
            }

            return Result.Success(await MapDetailsAsync(payment, cancellationToken));
        }

        if (payment.Status == PaymentStatuses.Succeeded && status == PaymentStatuses.Succeeded)
        {
            var ledgerResult = await RecordSuccessfulPaymentLedgerAsync(payment, cancellationToken);
            if (ledgerResult.IsFailure) return Result.Failure<PaymentDetailsResponse>(ledgerResult.ErrorCode!, ledgerResult.ErrorMessage!);
            return Result.Success(await MapDetailsAsync(payment, cancellationToken));
        }

        payment.ProviderTransactionId ??= providerTransactionId;
        payment.Status = status;
        payment.UpdatedAtUtc = DateTime.UtcNow;
        payment.FailureReason = failureReason;
        if (status == PaymentStatuses.Succeeded) payment.PaidAtUtc ??= DateTime.UtcNow;
        if (status == PaymentStatuses.Failed) payment.FailedAtUtc ??= DateTime.UtcNow;

        await repository.AddTransactionAsync(new PaymentTransaction { PaymentId = payment.Id, Provider = payment.Provider, ProviderTransactionId = providerTransactionId, ProviderOrderId = payment.ProviderOrderId, TransactionType = transactionType, Amount = payment.Amount, Currency = payment.Currency, Status = status, RawPayloadJson = rawPayloadJson, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);

        if (status == PaymentStatuses.Succeeded)
        {
            var jobResult = await jobPaymentStatusService.MarkJobPaidAsync(payment.JobId, payment.Id, null, cancellationToken);
            RefundRequest? refundRequest = null;
            if (jobResult.IsFailure && jobResult.ErrorCode == "job_cancelled")
            {
                refundRequest = new RefundRequest { PaymentId = payment.Id, JobId = payment.JobId, Amount = payment.Amount, Currency = payment.Currency, Reason = "Job was cancelled before payment confirmation", RequestedByUserId = payment.CustomerUserId, CreatedAtUtc = DateTime.UtcNow };
                await repository.AddRefundRequestAsync(refundRequest, cancellationToken);
            }

            await repository.SaveChangesAsync(cancellationToken);
            var ledgerResult = await RecordSuccessfulPaymentLedgerAsync(payment, cancellationToken);
            if (ledgerResult.IsFailure) return Result.Failure<PaymentDetailsResponse>(ledgerResult.ErrorCode!, ledgerResult.ErrorMessage!);
            if (refundRequest is not null)
            {
                var refundLedgerResult = await refundLedgerService.RecordRefundRequestedAsync(refundRequest.Id, payment.Id, payment.JobId, payment.Amount, payment.Currency, refundRequest.Reason, cancellationToken);
                if (refundLedgerResult.IsFailure) return Result.Failure<PaymentDetailsResponse>(refundLedgerResult.ErrorCode!, refundLedgerResult.ErrorMessage!);
            }
            await NotifyPaymentAsync(payment, NotificationTypes.PaymentSucceeded, "Payment succeeded", "Your payment was received.", cancellationToken);
        }
        else
        {
            await jobPaymentStatusService.MarkJobPaymentFailedAsync(payment.JobId, payment.Id, failureReason ?? "Payment failed.", cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            await NotifyPaymentAsync(payment, NotificationTypes.PaymentFailed, "Payment failed", "Your payment could not be completed.", cancellationToken);
        }
        return Result.Success(await MapDetailsAsync(payment, cancellationToken));
    }

    private async Task NotifyPaymentAsync(Payment payment, string type, string title, string body, CancellationToken cancellationToken)
    {
        try
        {
            await notificationService.SendAsync(new SendNotificationRequest(
                payment.CustomerUserId,
                type,
                title,
                body,
                new Dictionary<string, string?>
                {
                    ["paymentId"] = payment.Id.ToString(),
                    ["jobId"] = payment.JobId.ToString(),
                    ["jobNumber"] = payment.JobNumber,
                    ["amount"] = payment.Amount.ToString("0.00"),
                    ["currency"] = payment.Currency,
                    ["status"] = payment.Status
                },
                [NotificationChannels.InApp]), cancellationToken);
        }
        catch
        {
            // Notification failures must not break payment flow.
        }
    }

    private async Task<Result> RecordSuccessfulPaymentLedgerAsync(Payment payment, CancellationToken cancellationToken)
    {
        return await paymentLedgerService.RecordPaymentCapturedAsync(new PaymentCapturedLedgerRequest(
            payment.Id,
            payment.JobId,
            payment.PriceSnapshotId,
            payment.CustomerUserId,
            payment.Amount,
            payment.Currency,
            payment.PaidAtUtc ?? DateTime.UtcNow), cancellationToken);
    }

    private async Task<PaymentDetailsResponse> MapDetailsAsync(Payment payment, CancellationToken cancellationToken)
    {
        var transactions = await repository.GetTransactionsAsync(payment.Id, cancellationToken);
        return new PaymentDetailsResponse(payment.Id, payment.JobId, payment.JobNumber, payment.CustomerUserId, payment.PriceSnapshotId, payment.Amount, payment.Currency, payment.Provider, payment.Status, payment.CheckoutUrl, payment.ProviderOrderId, payment.ProviderTransactionId, payment.FailureReason, payment.CreatedAtUtc, payment.UpdatedAtUtc, payment.PaidAtUtc, payment.FailedAtUtc, transactions.Select(MapTransaction).ToArray());
    }

    private static PaymentCheckoutResponse MapCheckout(Payment payment) => new(payment.Id, payment.JobId, payment.Amount, payment.Currency, payment.Status, payment.Provider, payment.CheckoutUrl);
    private static PaymentSummaryResponse MapSummary(Payment payment) => new(payment.Id, payment.JobId, payment.JobNumber, payment.Amount, payment.Currency, payment.Provider, payment.Status, payment.CreatedAtUtc, payment.PaidAtUtc, payment.FailedAtUtc);
    private static PaymentTransactionResponse MapTransaction(PaymentTransaction transaction) => new(transaction.Id, transaction.Provider, transaction.ProviderTransactionId, transaction.ProviderOrderId, transaction.TransactionType, transaction.Amount, transaction.Currency, transaction.Status, transaction.CreatedAtUtc);
    private static PaymentWebhookLogResponse MapWebhookLog(PaymentWebhookLog log) => new(log.Id, log.Provider, log.EventType, log.ProviderTransactionId, log.ProviderOrderId, log.IsProcessed, log.ProcessingError, log.ReceivedAtUtc, log.ProcessedAtUtc);
}
