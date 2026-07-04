using Enjaz.Jobs.Application.Jobs;
using Enjaz.Jobs.Domain.Jobs;
using Enjaz.Jobs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class JobsRepository(JobsDbContext dbContext) : IJobsRepository
{
    public IQueryable<Job> QueryJobs() => dbContext.Jobs;

    public async Task<Job?> GetJobAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs.FirstOrDefaultAsync(job => job.Id == id, cancellationToken);
    }

    public async Task AddJobAsync(Job job, CancellationToken cancellationToken = default)
    {
        await dbContext.Jobs.AddAsync(job, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JobMedia>> GetMediaAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobMedia.AsNoTracking().Where(media => media.JobId == jobId).OrderBy(media => media.CreatedAtUtc).ToArrayAsync(cancellationToken);
    }

    public async Task AddMediaAsync(JobMedia media, CancellationToken cancellationToken = default)
    {
        await dbContext.JobMedia.AddAsync(media, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JobNote>> GetNotesAsync(Guid jobId, bool includeInternal, CancellationToken cancellationToken = default)
    {
        var query = dbContext.JobNotes.AsNoTracking().Where(note => note.JobId == jobId);
        if (!includeInternal)
        {
            query = query.Where(note => !note.IsInternal);
        }

        return await query.OrderBy(note => note.CreatedAtUtc).ToArrayAsync(cancellationToken);
    }

    public async Task AddNoteAsync(JobNote note, CancellationToken cancellationToken = default)
    {
        await dbContext.JobNotes.AddAsync(note, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JobStatusHistory>> GetStatusHistoryAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobStatusHistory.AsNoTracking().Where(history => history.JobId == jobId).OrderBy(history => history.CreatedAtUtc).ToArrayAsync(cancellationToken);
    }

    public async Task AddStatusHistoryAsync(JobStatusHistory history, CancellationToken cancellationToken = default)
    {
        await dbContext.JobStatusHistory.AddAsync(history, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JobAssignment>> GetAssignmentsAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobAssignments.AsNoTracking().Where(assignment => assignment.JobId == jobId).OrderByDescending(assignment => assignment.OfferedAtUtc).ToArrayAsync(cancellationToken);
    }

    public async Task<JobAssignment?> GetActiveAssignmentAsync(Guid jobId, Guid technicianUserId, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobAssignments
            .Where(assignment => assignment.JobId == jobId && assignment.TechnicianUserId == technicianUserId && assignment.Status == JobAssignmentStatuses.Offered)
            .OrderByDescending(assignment => assignment.OfferedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAssignmentAsync(JobAssignment assignment, CancellationToken cancellationToken = default)
    {
        await dbContext.JobAssignments.AddAsync(assignment, cancellationToken);
    }

    public async Task<IReadOnlyCollection<JobAssignment>> GetExpiredOfferedAssignmentsAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobAssignments
            .Where(assignment => assignment.Status == JobAssignmentStatuses.Offered && assignment.ExpiresAtUtc <= nowUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> OperationAlertExistsAsync(Guid jobId, string alertType, CancellationToken cancellationToken = default)
    {
        return await dbContext.JobOperationAlerts.AnyAsync(alert => alert.JobId == jobId && alert.AlertType == alertType && !alert.IsResolved, cancellationToken);
    }

    public async Task AddOperationAlertAsync(JobOperationAlert alert, CancellationToken cancellationToken = default)
    {
        await dbContext.JobOperationAlerts.AddAsync(alert, cancellationToken);
    }

    public async Task<string> GenerateJobNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var yearMonth = nowUtc.ToString("yyyyMM");
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var counter = await dbContext.JobCounters.FirstOrDefaultAsync(item => item.YearMonth == yearMonth, cancellationToken);
        if (counter is null)
        {
            counter = new JobCounter { YearMonth = yearMonth, LastNumber = 0, UpdatedAtUtc = nowUtc };
            await dbContext.JobCounters.AddAsync(counter, cancellationToken);
        }

        counter.LastNumber++;
        counter.UpdatedAtUtc = nowUtc;
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return $"ENJ-{yearMonth}-{counter.LastNumber:000000}";
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
