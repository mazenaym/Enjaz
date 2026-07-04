using Enjaz.Jobs.Domain.Jobs;
using Enjaz.Jobs.Infrastructure.Persistence;
using Enjaz.Maps.Application.Maps;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class JobExecutionLookupService(JobsDbContext dbContext) : IJobExecutionLookupService
{
    private static readonly string[] ActiveTrackingStatuses =
    [
        JobStatuses.TechnicianAccepted,
        JobStatuses.TechnicianOnWay,
        JobStatuses.Arrived,
        JobStatuses.InProgress
    ];

    public async Task<TechnicianActiveJobLookupResult?> GetActiveJobForTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .Where(job => job.AssignedTechnicianId == technicianId && ActiveTrackingStatuses.Contains(job.Status) && job.AssignedTechnicianUserId.HasValue)
            .OrderByDescending(job => job.UpdatedAtUtc ?? job.CreatedAtUtc)
            .Select(job => new TechnicianActiveJobLookupResult(job.Id, job.CustomerUserId, job.AssignedTechnicianId!.Value, job.AssignedTechnicianUserId!.Value, job.Status))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
