using Enjaz.Jobs.Application.Jobs;
using Enjaz.Payments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Payments.Infrastructure.Payments;

public sealed class JobPaymentSummaryLookupService(PaymentsDbContext dbContext) : IJobPaymentSummaryLookupService
{
    public async Task<JobPaymentSummaryResponse?> GetPaymentSummaryAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Payments
            .AsNoTracking()
            .Where(payment => payment.JobId == jobId)
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .Select(payment => new JobPaymentSummaryResponse(payment.Id, payment.Status, payment.Amount, payment.Currency, payment.PaidAtUtc, payment.FailedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
