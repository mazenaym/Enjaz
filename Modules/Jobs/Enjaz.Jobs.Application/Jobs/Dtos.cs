namespace Enjaz.Jobs.Application.Jobs;

public sealed record CreateJobRequest(
    Guid CustomerAddressId,
    Guid ServiceCategoryId,
    Guid ServiceId,
    Guid? ServiceTierId,
    Guid? AiClassificationId,
    Guid PriceSnapshotId,
    string? Title,
    string Description,
    DateTime? ScheduledAtUtc,
    DateTime? PreferredTimeWindowStartUtc,
    DateTime? PreferredTimeWindowEndUtc,
    IReadOnlyCollection<JobMediaRequest>? Media);

public sealed record JobMediaRequest(string MediaType, string FileUrl, string? FileKey, string? Caption);

public sealed record CancelJobRequest(string Reason);

public sealed record AdminUpdateJobStatusRequest(string Status, string? Reason);

public sealed record AdminAddJobNoteRequest(string Text, bool IsInternal);

public sealed record AssignTechnicianRequest(Guid TechnicianId, int ExpiresInMinutes = 15);

public sealed record RejectAssignmentRequest(string Reason);

public sealed record CompleteJobRequest(string? CompletionNotes, IReadOnlyCollection<JobMediaRequest>? Media);

public sealed record AdminForceCompleteRequest(string Reason);

public sealed record DisputeRequest(string Reason);

public sealed record JobListQuery(string? Status, int Page = 1, int PageSize = 20);

public sealed record AdminJobListQuery(
    string? Status,
    Guid? CustomerUserId,
    Guid? ServiceId,
    DateTime? FromDateUtc,
    DateTime? ToDateUtc,
    int Page = 1,
    int PageSize = 20);

public sealed record JobSummaryResponse(
    Guid Id,
    string JobNumber,
    string Status,
    Guid ServiceId,
    string DescriptionSummary,
    decimal EstimatedTotalAmount,
    DateTime? ScheduledAtUtc,
    DateTime CreatedAtUtc);

public sealed record JobDetailsResponse(
    Guid Id,
    string JobNumber,
    Guid CustomerUserId,
    Guid? CustomerProfileId,
    Guid CustomerAddressId,
    Guid ServiceCategoryId,
    Guid ServiceId,
    Guid? ServiceTierId,
    Guid? AiClassificationId,
    Guid PriceSnapshotId,
    Guid? ServiceZoneId,
    Guid? AssignedTechnicianId,
    Guid? AssignedTechnicianUserId,
    string? Title,
    string Description,
    string Status,
    DateTime? ScheduledAtUtc,
    DateTime? PreferredTimeWindowStartUtc,
    DateTime? PreferredTimeWindowEndUtc,
    string Currency,
    decimal EstimatedTotalAmount,
    decimal EstimatedDepositAmount,
    bool RequiresInspection,
    string? CancellationReason,
    DateTime? CancelledAtUtc,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyCollection<JobMediaResponse> Media,
    IReadOnlyCollection<JobNoteResponse> Notes,
    IReadOnlyCollection<JobStatusHistoryResponse> StatusHistory,
    IReadOnlyCollection<JobAssignmentResponse> Assignments);

public sealed record JobCreateResponse(
    Guid Id,
    string JobNumber,
    string Status,
    Guid PriceSnapshotId,
    decimal EstimatedTotalAmount,
    decimal EstimatedDepositAmount,
    string Currency,
    bool RequiresInspection);

public sealed record JobMediaResponse(Guid Id, string MediaType, string FileUrl, string? FileKey, string? Caption, Guid UploadedByUserId, DateTime CreatedAtUtc);

public sealed record JobNoteResponse(Guid Id, Guid AuthorUserId, string AuthorRole, string NoteType, string Text, bool IsInternal, DateTime CreatedAtUtc);

public sealed record JobStatusHistoryResponse(Guid Id, string? FromStatus, string ToStatus, Guid? ChangedByUserId, string? Reason, DateTime CreatedAtUtc);

public sealed record JobAssignmentResponse(Guid Id, Guid TechnicianId, Guid TechnicianUserId, string Status, DateTime OfferedAtUtc, DateTime? RespondedAtUtc, DateTime? ExpiresAtUtc, string? RejectionReason);

public sealed record TechnicianPublicProfileResponse(Guid TechnicianId, Guid UserId, string FullName, string? ProfileImageUrl, decimal AverageRating, int TotalReviews);

public sealed record TechnicianLocationResponse(Guid TechnicianId, decimal Latitude, decimal Longitude, DateTime UpdatedAtUtc);

public sealed record JobTrackingResponse(Guid JobId, string JobNumber, string Status, TechnicianPublicProfileResponse? AssignedTechnician, TechnicianLocationResponse? LatestTechnicianLocation, DateTime? LastLocationUpdatedAtUtc, Guid? ServiceZoneId, IReadOnlyCollection<JobStatusHistoryResponse> Timeline);

public sealed record JobTimelineResponse(Guid JobId, string JobNumber, IReadOnlyCollection<JobStatusHistoryResponse> StatusHistory, IReadOnlyCollection<JobNoteResponse> Notes, IReadOnlyCollection<JobMediaResponse> Media);

public sealed record JobPaymentSummaryResponse(Guid PaymentId, string Status, decimal Amount, string Currency, DateTime? PaidAtUtc, DateTime? FailedAtUtc);

public sealed record JobOperationsDetailsResponse(JobDetailsResponse Job, TechnicianPublicProfileResponse? AssignedTechnician, TechnicianLocationResponse? LatestTechnicianLocation, JobPaymentSummaryResponse? Payment, JobTimelineResponse Timeline);
