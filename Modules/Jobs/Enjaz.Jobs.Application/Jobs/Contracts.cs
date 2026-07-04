using Enjaz.Jobs.Domain.Jobs;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Jobs.Application.Jobs;

public interface ICustomerJobsService
{
    Task<Result<JobCreateResponse>> CreateAsync(CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<JobSummaryResponse>>> GetMyJobsAsync(JobListQuery query, CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> GetMyJobDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> CancelAsync(Guid id, CancelJobRequest request, CancellationToken cancellationToken = default);
    Task<Result<JobMediaResponse>> AddMediaAsync(Guid id, JobMediaRequest request, CancellationToken cancellationToken = default);
}

public interface IAdminJobsService
{
    Task<Result<IReadOnlyCollection<JobSummaryResponse>>> GetJobsAsync(AdminJobListQuery query, CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> GetJobDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> UpdateStatusAsync(Guid id, AdminUpdateJobStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<JobNoteResponse>> AddInternalNoteAsync(Guid id, AdminAddJobNoteRequest request, CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> AssignTechnicianAsync(Guid id, AssignTechnicianRequest request, CancellationToken cancellationToken = default);
}

public interface ITechnicianJobsService
{
    Task<Result<IReadOnlyCollection<JobSummaryResponse>>> GetMyAssignedJobsAsync(CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> AcceptAssignmentAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Result<JobDetailsResponse>> RejectAssignmentAsync(Guid jobId, RejectAssignmentRequest request, CancellationToken cancellationToken = default);
}

public interface IJobsRepository
{
    IQueryable<Job> QueryJobs();
    Task<Job?> GetJobAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddJobAsync(Job job, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<JobMedia>> GetMediaAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task AddMediaAsync(JobMedia media, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<JobNote>> GetNotesAsync(Guid jobId, bool includeInternal, CancellationToken cancellationToken = default);
    Task AddNoteAsync(JobNote note, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<JobStatusHistory>> GetStatusHistoryAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task AddStatusHistoryAsync(JobStatusHistory history, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<JobAssignment>> GetAssignmentsAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<JobAssignment?> GetActiveAssignmentAsync(Guid jobId, Guid technicianUserId, CancellationToken cancellationToken = default);
    Task AddAssignmentAsync(JobAssignment assignment, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<JobAssignment>> GetExpiredOfferedAssignmentsAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    Task<string> GenerateJobNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ICustomerLookupService
{
    Task<CustomerProfileLookupResult?> GetCustomerProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> AddressBelongsToCustomerAsync(Guid customerUserId, Guid addressId, CancellationToken cancellationToken = default);
    Task<CustomerAddressLocationResult?> GetCustomerAddressLocationAsync(Guid addressId, CancellationToken cancellationToken = default);
}

public interface IPricingSnapshotLookupService
{
    Task<PriceSnapshotLookupResult?> GetPriceSnapshotAsync(Guid priceSnapshotId, CancellationToken cancellationToken = default);
}

public interface IServiceZoneLookupService
{
    Task<ServiceZoneCoverageResult> CheckLocationCoverageAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
}

public interface IJobsEventPublisher
{
    Task PublishAsync(JobEventMessage message, CancellationToken cancellationToken = default);
}

public interface IExpireTechnicianAssignmentJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IJobPaymentLookupService
{
    Task<JobPaymentLookupResult?> GetPayableJobAsync(Guid jobId, Guid customerUserId, CancellationToken cancellationToken = default);
}

public interface IJobPaymentStatusService
{
    Task<Result> MarkJobPaidAsync(Guid jobId, Guid paymentId, Guid? changedByUserId, CancellationToken cancellationToken = default);
    Task<Result> MarkJobPaymentFailedAsync(Guid jobId, Guid paymentId, string reason, CancellationToken cancellationToken = default);
}

public sealed record CustomerProfileLookupResult(Guid Id, Guid UserId);
public sealed record CustomerAddressLocationResult(decimal? Latitude, decimal? Longitude);
public sealed record PriceSnapshotLookupResult(Guid Id, Guid? UserId, Guid ServiceCategoryId, Guid ServiceId, int ComplexityId, decimal BasePrice, decimal CommissionRate, decimal CommissionAmount, decimal VatRate, decimal VatAmount, decimal TotalAmount, decimal TechnicianPayoutAmount, decimal DepositAmount, string Currency, bool RequiresInspection, DateTime? ExpiresAtUtc);
public sealed record ServiceZoneCoverageResult(bool IsCovered, Guid? ServiceZoneId);
public sealed record JobEventMessage(string EventName, Guid JobId, Guid? CustomerUserId = null, Guid? TechnicianUserId = null, string? Status = null);
public sealed record JobPaymentLookupResult(Guid JobId, string JobNumber, Guid CustomerUserId, Guid PriceSnapshotId, string Status, decimal EstimatedTotalAmount, decimal EstimatedDepositAmount, string Currency, bool RequiresInspection);
