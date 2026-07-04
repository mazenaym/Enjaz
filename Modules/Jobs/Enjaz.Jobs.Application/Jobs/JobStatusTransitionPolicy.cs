using Enjaz.Jobs.Domain.Jobs;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Jobs.Application.Jobs;

public static class JobStatusTransitionPolicy
{
    public static Result CanTransition(string fromStatus, string toStatus, string actorRole)
    {
        if (!JobStatuses.All.Contains(toStatus))
        {
            return Result.Failure("invalid_job_status", "Job status is invalid.");
        }

        if (fromStatus == toStatus)
        {
            return Result.Success();
        }

        var allowed = (fromStatus, toStatus) switch
        {
            (JobStatuses.WaitingForPayment, JobStatuses.Cancelled) => true,
            (JobStatuses.WaitingForPayment, JobStatuses.TechnicianAssigned) => actorRole == JobNoteAuthorRoles.Admin,
            (JobStatuses.WaitingForPayment, JobStatuses.Paid) => actorRole is JobNoteAuthorRoles.System or JobNoteAuthorRoles.Admin,
            (JobStatuses.Paid, JobStatuses.SearchingTechnician) => actorRole is JobNoteAuthorRoles.System or JobNoteAuthorRoles.Admin,
            (JobStatuses.PendingInspectionPricing, JobStatuses.Cancelled) => true,
            (JobStatuses.PendingInspectionPricing, JobStatuses.WaitingForPayment) => actorRole == JobNoteAuthorRoles.Admin,
            (JobStatuses.TechnicianAssigned, JobStatuses.TechnicianAccepted) => true,
            (JobStatuses.TechnicianAssigned, JobStatuses.Cancelled) => true,
            (JobStatuses.TechnicianAssigned, JobStatuses.WaitingForManualAssignment) => true,
            (JobStatuses.WaitingForManualAssignment, JobStatuses.TechnicianAssigned) => actorRole == JobNoteAuthorRoles.Admin,
            (JobStatuses.WaitingForManualAssignment, JobStatuses.Cancelled) => true,
            (JobStatuses.TechnicianAccepted, JobStatuses.TechnicianOnWay) => true,
            (JobStatuses.TechnicianAccepted, JobStatuses.Cancelled) => actorRole == JobNoteAuthorRoles.Admin,
            (JobStatuses.TechnicianOnWay, JobStatuses.Arrived) => true,
            (JobStatuses.Arrived, JobStatuses.InProgress) => true,
            (JobStatuses.InProgress, JobStatuses.Completed) => true,
            _ => false
        };

        return allowed
            ? Result.Success()
            : Result.Failure("invalid_job_status_transition", $"Cannot transition job from {fromStatus} to {toStatus}.");
    }
}
