using System.Text.Json;
using Enjaz.Calls.Domain.Calls;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Calls.Application.Calls;

public sealed class CallsService(ICallsRepository repository, ICallJobLookupService jobLookupService, IIdentityUserService identityUserService, ICurrentUserContext currentUserContext, ICallProvider callProvider) : ICallsService
{
    private static readonly IReadOnlySet<string> ActiveStatuses = new HashSet<string> { "TechnicianAssigned", "TechnicianAccepted", "TechnicianOnWay", "Arrived", "InProgress" };

    public async Task<Result<CallSessionResponse>> StartCallAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId == Guid.Empty) return Result.Failure<CallSessionResponse>("unauthorized", "Authentication is required.");
        var job = await jobLookupService.GetCallableJobAsync(jobId, cancellationToken);
        if (job is null) return Result.Failure<CallSessionResponse>("job_not_found", "Job was not found.");
        if (!job.AssignedTechnicianId.HasValue || !job.AssignedTechnicianUserId.HasValue) return Result.Failure<CallSessionResponse>("job_not_assigned", "Job does not have an assigned technician.");
        if (!ActiveStatuses.Contains(job.Status)) return Result.Failure<CallSessionResponse>("job_not_callable", "Calls are only available for active assigned jobs.");
        if (job.CustomerUserId != currentUserContext.UserId && job.AssignedTechnicianUserId != currentUserContext.UserId) return Result.Failure<CallSessionResponse>("forbidden", "Only job participants can start a masked call.");

        var customer = await identityUserService.GetUserInfoAsync(job.CustomerUserId, cancellationToken);
        if (!customer.IsSuccess) return Result.Failure<CallSessionResponse>(customer.ErrorCode ?? "customer_not_found", customer.ErrorMessage ?? "Customer was not found.");
        var technician = await identityUserService.GetUserInfoAsync(job.AssignedTechnicianUserId.Value, cancellationToken);
        if (!technician.IsSuccess) return Result.Failure<CallSessionResponse>(technician.ErrorCode ?? "technician_not_found", technician.ErrorMessage ?? "Technician was not found.");
        var customerInfo = customer.Value!;
        var technicianInfo = technician.Value!;

        var now = DateTime.UtcNow;
        var session = new CallSession { JobId = job.JobId, CustomerUserId = job.CustomerUserId, TechnicianUserId = job.AssignedTechnicianUserId.Value, TechnicianId = job.AssignedTechnicianId.Value, InitiatedByUserId = currentUserContext.UserId, Provider = callProvider.ProviderName, Status = CallStatuses.Created, CreatedAtUtc = now };
        await repository.AddSessionAsync(session, cancellationToken);

        var providerResult = await callProvider.CreateMaskedCallAsync(new CreateMaskedCallRequest(job.JobId, job.CustomerUserId, job.AssignedTechnicianUserId.Value, customerInfo.PhoneNumber, technicianInfo.PhoneNumber, currentUserContext.UserId), cancellationToken);
        if (!providerResult.IsSuccess)
        {
            session.Status = CallStatuses.Failed;
            session.UpdatedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Failure<CallSessionResponse>(providerResult.ErrorCode ?? "call_provider_failed", providerResult.ErrorMessage ?? "Call provider failed.");
        }

        var providerCall = providerResult.Value!;
        session.Provider = providerCall.Provider;
        session.ProviderCallId = providerCall.ProviderCallId;
        session.MaskedNumber = providerCall.MaskedNumber;
        session.Status = providerCall.Status;
        session.StartedAtUtc = DateTime.UtcNow;
        session.UpdatedAtUtc = session.StartedAtUtc;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(session));
    }

    public async Task<Result<IReadOnlyCollection<CallSessionResponse>>> GetJobCallsAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobLookupService.GetCallableJobAsync(jobId, cancellationToken);
        if (job is null) return Result.Failure<IReadOnlyCollection<CallSessionResponse>>("job_not_found", "Job was not found.");
        if (job.CustomerUserId != currentUserContext.UserId && job.AssignedTechnicianUserId != currentUserContext.UserId) return Result.Failure<IReadOnlyCollection<CallSessionResponse>>("forbidden", "Only job participants can view call sessions.");
        return Result.Success<IReadOnlyCollection<CallSessionResponse>>(await repository.QuerySessions().AsNoTracking().Where(call => call.JobId == jobId).OrderByDescending(call => call.CreatedAtUtc).Select(call => Map(call)).ToListAsync(cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<CallSessionResponse>>> GetCallsAsync(CancellationToken cancellationToken = default) =>
        Result.Success<IReadOnlyCollection<CallSessionResponse>>(await repository.QuerySessions().AsNoTracking().OrderByDescending(call => call.CreatedAtUtc).Take(300).Select(call => Map(call)).ToListAsync(cancellationToken));

    public async Task<Result<CallSessionResponse>> GetCallAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var call = await repository.GetSessionAsync(id, cancellationToken);
        return call is null ? Result.Failure<CallSessionResponse>("call_not_found", "Call session was not found.") : Result.Success(Map(call));
    }

    public async Task<Result<IReadOnlyCollection<CallWebhookLogResponse>>> GetWebhookLogsAsync(CancellationToken cancellationToken = default) =>
        Result.Success<IReadOnlyCollection<CallWebhookLogResponse>>(await repository.QueryWebhookLogs().AsNoTracking().OrderByDescending(log => log.ReceivedAtUtc).Take(300).Select(log => Map(log)).ToListAsync(cancellationToken));

    public async Task<Result<CallWebhookLogResponse>> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var log = await repository.GetWebhookLogAsync(id, cancellationToken);
        return log is null ? Result.Failure<CallWebhookLogResponse>("webhook_not_found", "Webhook log was not found.") : Result.Success(Map(log));
    }

    public async Task<Result<CallWebhookLogResponse>> ProcessCalleraWebhookAsync(CallWebhookProcessRequest request, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Deserialize<Dictionary<string, object?>>(request.RawPayloadJson) ?? new Dictionary<string, object?>();
        var providerCallId = payload.GetValueOrDefault("providerCallId")?.ToString() ?? payload.GetValueOrDefault("callId")?.ToString();
        var eventType = payload.GetValueOrDefault("eventType")?.ToString() ?? payload.GetValueOrDefault("status")?.ToString();
        var log = new CallWebhookLog { Provider = CallProviders.Callera, ProviderCallId = providerCallId, EventType = eventType, RawPayloadJson = request.RawPayloadJson, HeadersJson = JsonSerializer.Serialize(request.Headers), ReceivedAtUtc = DateTime.UtcNow };
        await repository.AddWebhookLogAsync(log, cancellationToken);

        try
        {
            if (!string.IsNullOrWhiteSpace(providerCallId))
            {
                var session = await repository.GetSessionByProviderCallIdAsync(providerCallId, cancellationToken);
                if (session is not null)
                {
                    session.Status = NormalizeStatus(payload.GetValueOrDefault("status")?.ToString() ?? eventType);
                    session.DurationSeconds = int.TryParse(payload.GetValueOrDefault("durationSeconds")?.ToString(), out var duration) ? duration : session.DurationSeconds;
                    session.RecordingUrl = payload.GetValueOrDefault("recordingUrl")?.ToString() ?? session.RecordingUrl;
                    session.EndedAtUtc = session.Status is CallStatuses.Completed or CallStatuses.Failed or CallStatuses.Missed ? DateTime.UtcNow : session.EndedAtUtc;
                    session.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            log.IsProcessed = true;
            log.ProcessedAtUtc = DateTime.UtcNow;
        }
        catch (Exception exception)
        {
            log.ProcessingError = exception.Message;
        }

        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(log));
    }

    private static string NormalizeStatus(string? status) => status switch
    {
        CallStatuses.Ringing or "ringing" => CallStatuses.Ringing,
        CallStatuses.Connected or "connected" => CallStatuses.Connected,
        CallStatuses.Completed or "completed" => CallStatuses.Completed,
        CallStatuses.Failed or "failed" => CallStatuses.Failed,
        CallStatuses.Missed or "missed" => CallStatuses.Missed,
        _ => CallStatuses.Created
    };

    private static CallSessionResponse Map(CallSession call) => new(call.Id, call.JobId, call.CustomerUserId, call.TechnicianUserId, call.TechnicianId, call.InitiatedByUserId, call.Provider, call.ProviderCallId, call.MaskedNumber, call.Status, call.StartedAtUtc, call.EndedAtUtc, call.DurationSeconds, call.RecordingUrl, call.CreatedAtUtc);
    private static CallWebhookLogResponse Map(CallWebhookLog log) => new(log.Id, log.Provider, log.ProviderCallId, log.EventType, log.RawPayloadJson, log.HeadersJson, log.IsProcessed, log.ProcessingError, log.ReceivedAtUtc, log.ProcessedAtUtc);
}
