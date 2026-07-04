using Enjaz.Calls.Domain.Calls;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Calls.Application.Calls;

public interface ICallsService
{
    Task<Result<CallSessionResponse>> StartCallAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CallSessionResponse>>> GetJobCallsAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CallSessionResponse>>> GetCallsAsync(CancellationToken cancellationToken = default);
    Task<Result<CallSessionResponse>> GetCallAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<CallWebhookLogResponse>>> GetWebhookLogsAsync(CancellationToken cancellationToken = default);
    Task<Result<CallWebhookLogResponse>> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<CallWebhookLogResponse>> ProcessCalleraWebhookAsync(CallWebhookProcessRequest request, CancellationToken cancellationToken = default);
}

public interface ICallsRepository
{
    IQueryable<CallSession> QuerySessions();
    IQueryable<CallWebhookLog> QueryWebhookLogs();
    Task<CallSession?> GetSessionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CallSession?> GetSessionByProviderCallIdAsync(string providerCallId, CancellationToken cancellationToken = default);
    Task<CallWebhookLog?> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddSessionAsync(CallSession session, CancellationToken cancellationToken = default);
    Task AddWebhookLogAsync(CallWebhookLog log, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ICallJobLookupService
{
    Task<CallJobLookupResult?> GetCallableJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}

public interface ICallProvider
{
    string ProviderName { get; }
    Task<Result<CreateMaskedCallResult>> CreateMaskedCallAsync(CreateMaskedCallRequest request, CancellationToken cancellationToken = default);
}
