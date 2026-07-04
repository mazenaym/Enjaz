using Enjaz.Calls.Application.Calls;
using Enjaz.Jobs.Infrastructure.Persistence;
using Enjaz.Reviews.Application.Reviews;
using Enjaz.Support.Application.Support;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class Sprint10JobLookupService(JobsDbContext dbContext) : IReviewJobLookupService, ISupportJobLookupService, ICallJobLookupService
{
    public Task<ReviewJobLookupResult?> GetReviewableJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return dbContext.Jobs.AsNoTracking()
            .Where(job => job.Id == jobId)
            .Select(job => new ReviewJobLookupResult(job.Id, job.CustomerUserId, job.AssignedTechnicianId, job.AssignedTechnicianUserId, job.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<SupportJobLookupResult?> GetSupportJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return dbContext.Jobs.AsNoTracking()
            .Where(job => job.Id == jobId)
            .Select(job => new SupportJobLookupResult(job.Id, job.CustomerUserId, job.AssignedTechnicianUserId, job.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<CallJobLookupResult?> GetCallableJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return dbContext.Jobs.AsNoTracking()
            .Where(job => job.Id == jobId)
            .Select(job => new CallJobLookupResult(job.Id, job.CustomerUserId, job.AssignedTechnicianId, job.AssignedTechnicianUserId, job.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
