using Enjaz.Jobs.Domain.Jobs;

namespace Enjaz.Jobs.Application.Jobs;

public sealed class ExpireTechnicianAssignmentJob(IJobsRepository repository, IJobsEventPublisher eventPublisher) : IExpireTechnicianAssignmentJob
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var assignments = await repository.GetExpiredOfferedAssignmentsAsync(now, cancellationToken);

        foreach (var assignment in assignments)
        {
            assignment.Status = JobAssignmentStatuses.Expired;
            assignment.RespondedAtUtc = now;

            var job = await repository.GetJobAsync(assignment.JobId, cancellationToken);
            if (job is null || job.Status != JobStatuses.TechnicianAssigned || job.AssignedTechnicianId != assignment.TechnicianId)
            {
                continue;
            }

            var fromStatus = job.Status;
            job.AssignedTechnicianId = null;
            job.AssignedTechnicianUserId = null;
            job.Status = JobStatuses.WaitingForManualAssignment;
            job.UpdatedAtUtc = now;

            await repository.AddStatusHistoryAsync(new JobStatusHistory
            {
                JobId = job.Id,
                FromStatus = fromStatus,
                ToStatus = job.Status,
                Reason = "Technician assignment expired.",
                CreatedAtUtc = now
            }, cancellationToken);

            await repository.AddNoteAsync(new JobNote
            {
                JobId = job.Id,
                AuthorUserId = Guid.Empty,
                AuthorRole = JobNoteAuthorRoles.System,
                NoteType = JobNoteTypes.Assignment,
                Text = "Technician assignment expired.",
                IsInternal = true,
                CreatedAtUtc = now
            }, cancellationToken);

            await eventPublisher.PublishAsync(new JobEventMessage("job.status.changed", job.Id, job.CustomerUserId, assignment.TechnicianUserId, job.Status), cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
    }
}
