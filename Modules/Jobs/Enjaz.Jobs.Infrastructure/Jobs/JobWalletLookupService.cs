using Enjaz.Jobs.Infrastructure.Persistence;
using Enjaz.Wallets.Application.Wallets;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class JobWalletLookupService(JobsDbContext dbContext) : IJobWalletLookupService
{
    public async Task<JobWalletLookupResult?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .Where(job => job.Id == jobId)
            .Select(job => new JobWalletLookupResult(
                job.Id,
                job.JobNumber,
                job.CustomerUserId,
                job.PriceSnapshotId,
                job.AssignedTechnicianId,
                job.AssignedTechnicianUserId,
                job.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
