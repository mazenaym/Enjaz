using Enjaz.Jobs.Domain.Jobs;
using Enjaz.Maps.Application.Maps;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Application.Jobs;

public sealed class JobsService(
    IJobsRepository repository,
    ICurrentUserContext currentUserContext,
    ICustomerLookupService customerLookupService,
    IPricingSnapshotLookupService pricingSnapshotLookupService,
    IServiceZoneLookupService serviceZoneLookupService,
    ITechnicianLookupService technicianLookupService,
    IJobsEventPublisher eventPublisher,
    INotificationService notificationService)
    : ICustomerJobsService, IAdminJobsService, ITechnicianJobsService, IJobPaymentLookupService, IJobPaymentStatusService
{
    public async Task<Result<JobCreateResponse>> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default)
    {
        var userId = currentUserContext.UserId;
        var profile = await customerLookupService.GetCustomerProfileByUserIdAsync(userId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<JobCreateResponse>("customer_profile_not_found", "Customer profile was not found.");
        }

        if (!await customerLookupService.AddressBelongsToCustomerAsync(userId, request.CustomerAddressId, cancellationToken))
        {
            return Result.Failure<JobCreateResponse>("customer_address_not_found", "Customer address was not found.");
        }

        var snapshot = await pricingSnapshotLookupService.GetPriceSnapshotAsync(request.PriceSnapshotId, cancellationToken);
        if (snapshot is null)
        {
            return Result.Failure<JobCreateResponse>("price_snapshot_not_found", "Price snapshot was not found.");
        }

        if (snapshot.UserId.HasValue && snapshot.UserId.Value != userId)
        {
            return Result.Failure<JobCreateResponse>("price_snapshot_not_owned", "Price snapshot does not belong to the current user.");
        }

        if (snapshot.ServiceCategoryId != request.ServiceCategoryId || snapshot.ServiceId != request.ServiceId)
        {
            return Result.Failure<JobCreateResponse>("price_snapshot_mismatch", "Price snapshot does not match the requested service.");
        }

        if (snapshot.ExpiresAtUtc.HasValue && snapshot.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            return Result.Failure<JobCreateResponse>("price_snapshot_expired", "Price snapshot has expired.");
        }

        Guid? serviceZoneId = null;
        var location = await customerLookupService.GetCustomerAddressLocationAsync(request.CustomerAddressId, cancellationToken);
        if (location?.Latitude is not null && location.Longitude is not null)
        {
            var coverage = await serviceZoneLookupService.CheckLocationCoverageAsync(location.Latitude.Value, location.Longitude.Value, cancellationToken);
            if (!coverage.IsCovered)
            {
                return Result.Failure<JobCreateResponse>("service_area_not_supported", "Customer address is outside the supported service area.");
            }

            serviceZoneId = coverage.ServiceZoneId;
        }

        var now = DateTime.UtcNow;
        var status = snapshot.RequiresInspection ? JobStatuses.PendingInspectionPricing : JobStatuses.WaitingForPayment;
        var job = new Job
        {
            JobNumber = await repository.GenerateJobNumberAsync(now, cancellationToken),
            CustomerUserId = userId,
            CustomerProfileId = profile.Id,
            CustomerAddressId = request.CustomerAddressId,
            ServiceCategoryId = request.ServiceCategoryId,
            ServiceId = request.ServiceId,
            ServiceTierId = request.ServiceTierId,
            AiClassificationId = request.AiClassificationId,
            PriceSnapshotId = request.PriceSnapshotId,
            ServiceZoneId = serviceZoneId,
            Title = request.Title?.Trim(),
            Description = request.Description.Trim(),
            Status = status,
            ScheduledAtUtc = request.ScheduledAtUtc,
            PreferredTimeWindowStartUtc = request.PreferredTimeWindowStartUtc,
            PreferredTimeWindowEndUtc = request.PreferredTimeWindowEndUtc,
            Currency = snapshot.Currency,
            EstimatedTotalAmount = snapshot.TotalAmount,
            EstimatedDepositAmount = snapshot.DepositAmount,
            RequiresInspection = snapshot.RequiresInspection,
            CreatedAtUtc = now
        };

        await repository.AddJobAsync(job, cancellationToken);
        await repository.AddStatusHistoryAsync(new JobStatusHistory { JobId = job.Id, ToStatus = status, ChangedByUserId = userId, Reason = "Job created.", CreatedAtUtc = now }, cancellationToken);
        await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = userId, AuthorRole = JobNoteAuthorRoles.System, NoteType = JobNoteTypes.General, Text = "Job created.", IsInternal = true, CreatedAtUtc = now }, cancellationToken);

        foreach (var media in request.Media ?? Array.Empty<JobMediaRequest>())
        {
            await repository.AddMediaAsync(MapMedia(job.Id, userId, media, now), cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.created", job.Id, job.CustomerUserId, Status: job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.JobCreated, "Job created", "Your job was created.", JobData(job), cancellationToken);

        return Result.Success(new JobCreateResponse(job.Id, job.JobNumber, job.Status, job.PriceSnapshotId, job.EstimatedTotalAmount, job.EstimatedDepositAmount, job.Currency, job.RequiresInspection));
    }

    public async Task<Result<IReadOnlyCollection<JobSummaryResponse>>> GetMyJobsAsync(JobListQuery query, CancellationToken cancellationToken = default)
    {
        var jobs = await ApplyPaging(ApplyStatus(repository.QueryJobs().AsNoTracking().Where(job => job.CustomerUserId == currentUserContext.UserId), query.Status), query.Page, query.PageSize)
            .Select(job => MapSummary(job))
            .ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<JobSummaryResponse>>(jobs);
    }

    public async Task<Result<JobDetailsResponse>> GetMyJobDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        if (job is null || job.CustomerUserId != currentUserContext.UserId)
        {
            return Result.Failure<JobDetailsResponse>("job_not_found", "Job was not found.");
        }

        return Result.Success(await MapDetailsAsync(job, includeInternal: false, includeAssignments: false, cancellationToken));
    }

    public async Task<Result<JobDetailsResponse>> CancelAsync(Guid id, CancelJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        if (job is null || job.CustomerUserId != currentUserContext.UserId)
        {
            return Result.Failure<JobDetailsResponse>("job_not_found", "Job was not found.");
        }

        var transition = JobStatusTransitionPolicy.CanTransition(job.Status, JobStatuses.Cancelled, JobNoteAuthorRoles.Customer);
        if (transition.IsFailure)
        {
            return Result.Failure<JobDetailsResponse>(transition.ErrorCode!, transition.ErrorMessage!);
        }

        await ChangeStatusAsync(job, JobStatuses.Cancelled, currentUserContext.UserId, request.Reason, cancellationToken);
        job.CancellationReason = request.Reason.Trim();
        job.CancelledByUserId = currentUserContext.UserId;
        job.CancelledAtUtc = DateTime.UtcNow;
        await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = currentUserContext.UserId, AuthorRole = JobNoteAuthorRoles.Customer, NoteType = JobNoteTypes.Cancellation, Text = request.Reason.Trim(), CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.cancelled", job.Id, job.CustomerUserId, Status: job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.JobCancelled, "Job cancelled", "Your job was cancelled.", JobData(job), cancellationToken);
        return Result.Success(await MapDetailsAsync(job, includeInternal: false, includeAssignments: false, cancellationToken));
    }

    public async Task<Result<JobMediaResponse>> AddMediaAsync(Guid id, JobMediaRequest request, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        if (job is null || job.CustomerUserId != currentUserContext.UserId)
        {
            return Result.Failure<JobMediaResponse>("job_not_found", "Job was not found.");
        }

        var media = MapMedia(job.Id, currentUserContext.UserId, request, DateTime.UtcNow);
        await repository.AddMediaAsync(media, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapMediaResponse(media));
    }

    public async Task<Result<IReadOnlyCollection<JobSummaryResponse>>> GetJobsAsync(AdminJobListQuery query, CancellationToken cancellationToken = default)
    {
        var jobsQuery = repository.QueryJobs().AsNoTracking();
        jobsQuery = ApplyStatus(jobsQuery, query.Status);
        if (query.CustomerUserId.HasValue) jobsQuery = jobsQuery.Where(job => job.CustomerUserId == query.CustomerUserId.Value);
        if (query.ServiceId.HasValue) jobsQuery = jobsQuery.Where(job => job.ServiceId == query.ServiceId.Value);
        if (query.FromDateUtc.HasValue) jobsQuery = jobsQuery.Where(job => job.CreatedAtUtc >= query.FromDateUtc.Value);
        if (query.ToDateUtc.HasValue) jobsQuery = jobsQuery.Where(job => job.CreatedAtUtc <= query.ToDateUtc.Value);

        var jobs = await ApplyPaging(jobsQuery, query.Page, query.PageSize).Select(job => MapSummary(job)).ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<JobSummaryResponse>>(jobs);
    }

    public async Task<Result<JobDetailsResponse>> GetJobDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        return job is null
            ? Result.Failure<JobDetailsResponse>("job_not_found", "Job was not found.")
            : Result.Success(await MapDetailsAsync(job, includeInternal: true, includeAssignments: true, cancellationToken));
    }

    public async Task<Result<JobDetailsResponse>> UpdateStatusAsync(Guid id, AdminUpdateJobStatusRequest request, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        if (job is null)
        {
            return Result.Failure<JobDetailsResponse>("job_not_found", "Job was not found.");
        }

        var transition = JobStatusTransitionPolicy.CanTransition(job.Status, request.Status, JobNoteAuthorRoles.Admin);
        if (transition.IsFailure)
        {
            return Result.Failure<JobDetailsResponse>(transition.ErrorCode!, transition.ErrorMessage!);
        }

        await ChangeStatusAsync(job, request.Status, currentUserContext.UserId, request.Reason, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.status.changed", job.Id, job.CustomerUserId, job.AssignedTechnicianUserId, job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.JobStatusChanged, "Job status changed", "Your job status changed.", JobData(job), cancellationToken);
        return Result.Success(await MapDetailsAsync(job, includeInternal: true, includeAssignments: true, cancellationToken));
    }

    public async Task<Result<JobNoteResponse>> AddInternalNoteAsync(Guid id, AdminAddJobNoteRequest request, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        if (job is null)
        {
            return Result.Failure<JobNoteResponse>("job_not_found", "Job was not found.");
        }

        var note = new JobNote { JobId = id, AuthorUserId = currentUserContext.UserId, AuthorRole = JobNoteAuthorRoles.Admin, NoteType = request.IsInternal ? JobNoteTypes.Internal : JobNoteTypes.General, Text = request.Text.Trim(), IsInternal = request.IsInternal, CreatedAtUtc = DateTime.UtcNow };
        await repository.AddNoteAsync(note, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapNote(note));
    }

    public async Task<Result<JobDetailsResponse>> AssignTechnicianAsync(Guid id, AssignTechnicianRequest request, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(id, cancellationToken);
        if (job is null)
        {
            return Result.Failure<JobDetailsResponse>("job_not_found", "Job was not found.");
        }

        var technician = await technicianLookupService.GetByTechnicianIdAsync(request.TechnicianId, cancellationToken);
        if (technician is null)
        {
            return Result.Failure<JobDetailsResponse>("technician_not_found", "Technician was not found.");
        }

        if (technician.Status != "Approved")
        {
            return Result.Failure<JobDetailsResponse>("technician_not_approved", "Technician is not approved.");
        }

        var transition = JobStatusTransitionPolicy.CanTransition(job.Status, JobStatuses.TechnicianAssigned, JobNoteAuthorRoles.Admin);
        if (transition.IsFailure)
        {
            return Result.Failure<JobDetailsResponse>(transition.ErrorCode!, transition.ErrorMessage!);
        }

        var now = DateTime.UtcNow;
        var assignment = new JobAssignment { JobId = id, TechnicianId = technician.TechnicianId, TechnicianUserId = technician.UserId, Status = JobAssignmentStatuses.Offered, OfferedAtUtc = now, ExpiresAtUtc = now.AddMinutes(request.ExpiresInMinutes) };
        await repository.AddAssignmentAsync(assignment, cancellationToken);
        job.AssignedTechnicianId = technician.TechnicianId;
        job.AssignedTechnicianUserId = technician.UserId;
        await ChangeStatusAsync(job, JobStatuses.TechnicianAssigned, currentUserContext.UserId, "Manual admin assignment.", cancellationToken);
        await repository.AddNoteAsync(new JobNote { JobId = id, AuthorUserId = currentUserContext.UserId, AuthorRole = JobNoteAuthorRoles.Admin, NoteType = JobNoteTypes.Assignment, Text = $"Technician assigned: {technician.TechnicianId}", IsInternal = true, CreatedAtUtc = now }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.assigned", job.Id, job.CustomerUserId, technician.UserId, job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.TechnicianAssigned, "Technician assigned", "A technician was assigned to your job.", JobData(job), cancellationToken);
        await NotifyAsync(technician.UserId, NotificationTypes.TechnicianAssigned, "New assignment", "You have a new assignment.", JobData(job), cancellationToken);
        return Result.Success(await MapDetailsAsync(job, includeInternal: true, includeAssignments: true, cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<JobSummaryResponse>>> GetMyAssignedJobsAsync(CancellationToken cancellationToken = default)
    {
        var jobs = await repository.QueryJobs()
            .AsNoTracking()
            .Where(job => job.AssignedTechnicianUserId == currentUserContext.UserId)
            .OrderByDescending(job => job.CreatedAtUtc)
            .Select(job => MapSummary(job))
            .ToArrayAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<JobSummaryResponse>>(jobs);
    }

    public async Task<Result<JobDetailsResponse>> AcceptAssignmentAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(jobId, cancellationToken);
        var assignment = await repository.GetActiveAssignmentAsync(jobId, currentUserContext.UserId, cancellationToken);
        if (job is null || assignment is null)
        {
            return Result.Failure<JobDetailsResponse>("assignment_not_found", "Assignment was not found.");
        }

        if (assignment.ExpiresAtUtc.HasValue && assignment.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            return Result.Failure<JobDetailsResponse>("assignment_expired", "Assignment has expired.");
        }

        assignment.Status = JobAssignmentStatuses.Accepted;
        assignment.RespondedAtUtc = DateTime.UtcNow;
        await ChangeStatusAsync(job, JobStatuses.TechnicianAccepted, currentUserContext.UserId, "Technician accepted assignment.", cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.assignment.accepted", job.Id, job.CustomerUserId, assignment.TechnicianUserId, job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.AssignmentAccepted, "Assignment accepted", "The technician accepted your job.", JobData(job), cancellationToken);
        await NotifyAsync(assignment.TechnicianUserId, NotificationTypes.AssignmentAccepted, "Assignment accepted", "You accepted the assignment.", JobData(job), cancellationToken);
        return Result.Success(await MapDetailsAsync(job, includeInternal: false, includeAssignments: true, cancellationToken));
    }

    public async Task<Result<JobDetailsResponse>> RejectAssignmentAsync(Guid jobId, RejectAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(jobId, cancellationToken);
        var assignment = await repository.GetActiveAssignmentAsync(jobId, currentUserContext.UserId, cancellationToken);
        if (job is null || assignment is null)
        {
            return Result.Failure<JobDetailsResponse>("assignment_not_found", "Assignment was not found.");
        }

        assignment.Status = JobAssignmentStatuses.Rejected;
        assignment.RespondedAtUtc = DateTime.UtcNow;
        assignment.RejectionReason = request.Reason.Trim();
        job.AssignedTechnicianId = null;
        job.AssignedTechnicianUserId = null;
        await ChangeStatusAsync(job, JobStatuses.WaitingForManualAssignment, currentUserContext.UserId, request.Reason, cancellationToken);
        await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = currentUserContext.UserId, AuthorRole = JobNoteAuthorRoles.Technician, NoteType = JobNoteTypes.Assignment, Text = request.Reason.Trim(), IsInternal = true, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.assignment.rejected", job.Id, job.CustomerUserId, assignment.TechnicianUserId, job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.AssignmentRejected, "Assignment rejected", "The technician rejected your job assignment.", JobData(job), cancellationToken);
        await NotifyAsync(assignment.TechnicianUserId, NotificationTypes.AssignmentRejected, "Assignment rejected", "You rejected the assignment.", JobData(job), cancellationToken);
        return Result.Success(await MapDetailsAsync(job, includeInternal: false, includeAssignments: true, cancellationToken));
    }

    public async Task<JobPaymentLookupResult?> GetPayableJobAsync(Guid jobId, Guid customerUserId, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(jobId, cancellationToken);
        if (job is null || job.CustomerUserId != customerUserId || job.Status != JobStatuses.WaitingForPayment || job.RequiresInspection)
        {
            return null;
        }

        return new JobPaymentLookupResult(job.Id, job.JobNumber, job.CustomerUserId, job.PriceSnapshotId, job.Status, job.EstimatedTotalAmount, job.EstimatedDepositAmount, job.Currency, job.RequiresInspection);
    }

    public async Task<Result> MarkJobPaidAsync(Guid jobId, Guid paymentId, Guid? changedByUserId, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(jobId, cancellationToken);
        if (job is null)
        {
            return Result.Failure("job_not_found", "Job was not found.");
        }

        if (job.Status == JobStatuses.Cancelled)
        {
            await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = changedByUserId ?? Guid.Empty, AuthorRole = JobNoteAuthorRoles.System, NoteType = JobNoteTypes.Pricing, Text = $"Payment {paymentId} succeeded after cancellation. Refund review required.", IsInternal = true, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Failure("job_cancelled", "Job was cancelled before payment confirmation.");
        }

        if (job.Status != JobStatuses.WaitingForPayment && job.Status != JobStatuses.Paid && job.Status != JobStatuses.SearchingTechnician)
        {
            return Result.Failure("job_not_payable", "Job cannot be marked paid from its current status.");
        }

        if (job.Status == JobStatuses.WaitingForPayment)
        {
            await ChangeStatusAsync(job, JobStatuses.Paid, changedByUserId, $"Payment {paymentId} succeeded.", cancellationToken);
        }

        if (job.Status == JobStatuses.Paid)
        {
            await ChangeStatusAsync(job, JobStatuses.SearchingTechnician, changedByUserId, "Payment complete. Searching for technician.", cancellationToken);
        }

        await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = changedByUserId ?? Guid.Empty, AuthorRole = JobNoteAuthorRoles.System, NoteType = JobNoteTypes.Pricing, Text = $"Payment {paymentId} succeeded.", IsInternal = true, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.payment.succeeded", job.Id, job.CustomerUserId, job.AssignedTechnicianUserId, job.Status), cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.status.changed", job.Id, job.CustomerUserId, job.AssignedTechnicianUserId, job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.PaymentSucceeded, "Payment succeeded", "Your payment was received.", JobData(job), cancellationToken);
        return Result.Success();
    }

    public async Task<Result> MarkJobPaymentFailedAsync(Guid jobId, Guid paymentId, string reason, CancellationToken cancellationToken = default)
    {
        var job = await repository.GetJobAsync(jobId, cancellationToken);
        if (job is null)
        {
            return Result.Failure("job_not_found", "Job was not found.");
        }

        await repository.AddNoteAsync(new JobNote { JobId = job.Id, AuthorUserId = Guid.Empty, AuthorRole = JobNoteAuthorRoles.System, NoteType = JobNoteTypes.Pricing, Text = $"Payment {paymentId} failed: {reason}", IsInternal = true, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await eventPublisher.PublishAsync(new JobEventMessage("job.payment.failed", job.Id, job.CustomerUserId, job.AssignedTechnicianUserId, job.Status), cancellationToken);
        await NotifyAsync(job.CustomerUserId, NotificationTypes.PaymentFailed, "Payment failed", "Your payment could not be completed.", JobData(job), cancellationToken);
        return Result.Success();
    }

    private async Task NotifyAsync(Guid userId, string type, string title, string body, IReadOnlyDictionary<string, string?> data, CancellationToken cancellationToken)
    {
        try
        {
            await notificationService.SendAsync(new SendNotificationRequest(userId, type, title, body, data, [NotificationChannels.InApp]), cancellationToken);
        }
        catch
        {
            // Notification delivery must not block the primary job flow.
        }
    }

    private static IReadOnlyDictionary<string, string?> JobData(Job job) => new Dictionary<string, string?>
    {
        ["jobId"] = job.Id.ToString(),
        ["jobNumber"] = job.JobNumber,
        ["status"] = job.Status,
        ["amount"] = job.EstimatedTotalAmount.ToString("0.00"),
        ["currency"] = job.Currency
    };

    private async Task ChangeStatusAsync(Job job, string toStatus, Guid? changedByUserId, string? reason, CancellationToken cancellationToken)
    {
        var fromStatus = job.Status;
        job.Status = toStatus;
        job.UpdatedAtUtc = DateTime.UtcNow;
        await repository.AddStatusHistoryAsync(new JobStatusHistory { JobId = job.Id, FromStatus = fromStatus, ToStatus = toStatus, ChangedByUserId = changedByUserId, Reason = reason, CreatedAtUtc = DateTime.UtcNow }, cancellationToken);
    }

    private async Task<JobDetailsResponse> MapDetailsAsync(Job job, bool includeInternal, bool includeAssignments, CancellationToken cancellationToken)
    {
        var media = await repository.GetMediaAsync(job.Id, cancellationToken);
        var notes = await repository.GetNotesAsync(job.Id, includeInternal, cancellationToken);
        var history = await repository.GetStatusHistoryAsync(job.Id, cancellationToken);
        var assignments = includeAssignments ? await repository.GetAssignmentsAsync(job.Id, cancellationToken) : Array.Empty<JobAssignment>();
        return new JobDetailsResponse(
            job.Id, job.JobNumber, job.CustomerUserId, job.CustomerProfileId, job.CustomerAddressId,
            job.ServiceCategoryId, job.ServiceId, job.ServiceTierId, job.AiClassificationId, job.PriceSnapshotId,
            job.ServiceZoneId, job.AssignedTechnicianId, job.AssignedTechnicianUserId, job.Title, job.Description,
            job.Status, job.ScheduledAtUtc, job.PreferredTimeWindowStartUtc, job.PreferredTimeWindowEndUtc,
            job.Currency, job.EstimatedTotalAmount, job.EstimatedDepositAmount, job.RequiresInspection,
            job.CancellationReason, job.CancelledAtUtc, job.CreatedAtUtc, job.UpdatedAtUtc,
            media.Select(MapMediaResponse).ToArray(), notes.Select(MapNote).ToArray(),
            history.Select(item => new JobStatusHistoryResponse(item.Id, item.FromStatus, item.ToStatus, item.ChangedByUserId, item.Reason, item.CreatedAtUtc)).ToArray(),
            assignments.Select(item => new JobAssignmentResponse(item.Id, item.TechnicianId, item.TechnicianUserId, item.Status, item.OfferedAtUtc, item.RespondedAtUtc, item.ExpiresAtUtc, item.RejectionReason)).ToArray());
    }

    private static IQueryable<Job> ApplyStatus(IQueryable<Job> query, string? status) => string.IsNullOrWhiteSpace(status) ? query : query.Where(job => job.Status == status);
    private static IQueryable<Job> ApplyPaging(IQueryable<Job> query, int page, int pageSize) => query.OrderByDescending(job => job.CreatedAtUtc).Skip((Math.Max(page, 1) - 1) * Math.Clamp(pageSize, 1, 100)).Take(Math.Clamp(pageSize, 1, 100));
    private static JobSummaryResponse MapSummary(Job job) => new(job.Id, job.JobNumber, job.Status, job.ServiceId, job.Description.Length <= 120 ? job.Description : job.Description[..120], job.EstimatedTotalAmount, job.ScheduledAtUtc, job.CreatedAtUtc);
    private static JobMedia MapMedia(Guid jobId, Guid userId, JobMediaRequest request, DateTime now) => new() { JobId = jobId, UploadedByUserId = userId, MediaType = request.MediaType, FileUrl = request.FileUrl.Trim(), FileKey = request.FileKey?.Trim(), Caption = request.Caption?.Trim(), CreatedAtUtc = now };
    private static JobMediaResponse MapMediaResponse(JobMedia media) => new(media.Id, media.MediaType, media.FileUrl, media.FileKey, media.Caption, media.UploadedByUserId, media.CreatedAtUtc);
    private static JobNoteResponse MapNote(JobNote note) => new(note.Id, note.AuthorUserId, note.AuthorRole, note.NoteType, note.Text, note.IsInternal, note.CreatedAtUtc);
}
