using Enjaz.Payments.Application.Payments;
using Enjaz.Payments.Domain.Payments;
using Enjaz.Payments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Payments.Infrastructure.Payments;

public sealed class PaymentsRepository(PaymentsDbContext dbContext) : IPaymentsRepository
{
    public IQueryable<Payment> QueryPayments() => dbContext.Payments;
    public async Task<Payment?> GetPaymentAsync(Guid id, CancellationToken cancellationToken = default) => await dbContext.Payments.FirstOrDefaultAsync(payment => payment.Id == id, cancellationToken);
    public async Task<Payment?> GetActivePaymentForJobAsync(Guid jobId, CancellationToken cancellationToken = default) => await dbContext.Payments.Where(payment => payment.JobId == jobId && PaymentStatuses.Active.Contains(payment.Status)).OrderByDescending(payment => payment.CreatedAtUtc).FirstOrDefaultAsync(cancellationToken);
    public async Task<Payment?> GetPaymentByProviderOrderIdAsync(string providerOrderId, CancellationToken cancellationToken = default) => await dbContext.Payments.FirstOrDefaultAsync(payment => payment.ProviderOrderId == providerOrderId, cancellationToken);
    public async Task<Payment?> GetPaymentByProviderTransactionIdAsync(string providerTransactionId, CancellationToken cancellationToken = default) => await dbContext.Payments.FirstOrDefaultAsync(payment => payment.ProviderTransactionId == providerTransactionId, cancellationToken);
    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default) => await dbContext.Payments.AddAsync(payment, cancellationToken);
    public async Task<IReadOnlyCollection<PaymentTransaction>> GetTransactionsAsync(Guid paymentId, CancellationToken cancellationToken = default) => await dbContext.PaymentTransactions.AsNoTracking().Where(transaction => transaction.PaymentId == paymentId).OrderBy(transaction => transaction.CreatedAtUtc).ToArrayAsync(cancellationToken);
    public async Task<bool> TransactionExistsAsync(string providerTransactionId, string transactionType, CancellationToken cancellationToken = default) => await dbContext.PaymentTransactions.AnyAsync(transaction => transaction.ProviderTransactionId == providerTransactionId && transaction.TransactionType == transactionType, cancellationToken);
    public async Task AddTransactionAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default) => await dbContext.PaymentTransactions.AddAsync(transaction, cancellationToken);
    public IQueryable<PaymentWebhookLog> QueryWebhookLogs() => dbContext.PaymentWebhookLogs;
    public async Task<PaymentWebhookLog?> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default) => await dbContext.PaymentWebhookLogs.AsNoTracking().FirstOrDefaultAsync(log => log.Id == id, cancellationToken);
    public async Task AddWebhookLogAsync(PaymentWebhookLog log, CancellationToken cancellationToken = default) => await dbContext.PaymentWebhookLogs.AddAsync(log, cancellationToken);
    public async Task AddRefundRequestAsync(RefundRequest refundRequest, CancellationToken cancellationToken = default) => await dbContext.RefundRequests.AddAsync(refundRequest, cancellationToken);
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await dbContext.SaveChangesAsync(cancellationToken);
}
