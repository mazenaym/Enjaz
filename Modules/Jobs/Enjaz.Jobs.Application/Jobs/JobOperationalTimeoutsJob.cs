using Enjaz.Jobs.Domain.Jobs;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Application.Jobs;

public sealed class JobOperationalTimeoutsJob(IJobsRepository repository, INotificationService notificationService) : IJobOperationalTimeoutsJob
{
    public async Task CheckTechnicianOnWayLateAsync(CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-60);
        var jobs = await repository.QueryJobs()
            .Where(job => job.Status == JobStatuses.TechnicianOnWay && (job.UpdatedAtUtc ?? job.CreatedAtUtc) <= cutoff)
            .Take(100)
            .ToArrayAsync(cancellationToken);

        foreach (var job in jobs)
        {
            await AddAlertAsync(job, JobOperationAlertTypes.TechnicianOnWayLate, "Technician has been on the way too long.", cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task CheckInProgressTooLongAsync(CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddHours(-4);
        var jobs = await repository.QueryJobs()
            .Where(job => job.Status == JobStatuses.InProgress && (job.UpdatedAtUtc ?? job.CreatedAtUtc) <= cutoff)
            .Take(100)
            .ToArrayAsync(cancellationToken);

        foreach (var job in jobs)
        {
            await AddAlertAsync(job, JobOperationAlertTypes.InProgressTooLong, "Job has been in progress too long.", cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task AddAlertAsync(Job job, string alertType, string message, CancellationToken cancellationToken)
    {
        if (await repository.OperationAlertExistsAsync(job.Id, alertType, cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        await repository.AddOperationAlertAsync(new JobOperationAlert { JobId = job.Id, AlertType = alertType, CreatedAtUtc = now }, cancellationToken);
        await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = Guid.Empty, AuthorRole = JobNoteAuthorRoles.System, NoteType = JobNoteTypes.Internal, Text = message, IsInternal = true, CreatedAtUtc = now }, cancellationToken);

        try
        {
            await notificationService.SendInAppAsync(Guid.Empty, NotificationTypes.General, "Admin intervention required", $"{message} Job {job.JobNumber}.", new Dictionary<string, string?> { ["jobId"] = job.Id.ToString(), ["jobNumber"] = job.JobNumber }, cancellationToken);
        }
        catch
        {
        }
    }
}
