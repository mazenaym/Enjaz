using Enjaz.SharedKernel.Results;
using Enjaz.Support.Domain.Support;

namespace Enjaz.Support.Application.Support;

public interface ISupportService
{
    Task<Result<SupportTicketResponse>> CreateTicketAsync(CreateSupportTicketRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<SupportTicketResponse>>> GetMyTicketsAsync(CancellationToken cancellationToken = default);
    Task<Result<SupportTicketResponse>> GetTicketAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SupportTicketMessageResponse>> AddMessageAsync(Guid id, AddTicketMessageRequest request, CancellationToken cancellationToken = default);
    Task<Result<JobDisputeResponse>> OpenDisputeAsync(Guid jobId, OpenDisputeRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<SupportTicketResponse>>> GetTicketsAsync(SupportTicketQuery query, CancellationToken cancellationToken = default);
    Task<Result<SupportTicketResponse>> GetTicketForAdminAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SupportTicketResponse>> AssignTicketAsync(Guid id, AdminAssignTicketRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupportTicketResponse>> UpdateTicketStatusAsync(Guid id, AdminUpdateTicketStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupportTicketMessageResponse>> AddAdminMessageAsync(Guid id, AddTicketMessageRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<JobDisputeResponse>>> GetDisputesAsync(JobDisputeQuery query, CancellationToken cancellationToken = default);
    Task<Result<JobDisputeResponse>> ResolveDisputeAsync(Guid id, ResolveDisputeRequest request, CancellationToken cancellationToken = default);
    Task<Result<JobDisputeResponse>> RejectDisputeAsync(Guid id, ResolveDisputeRequest request, CancellationToken cancellationToken = default);
}

public interface ISupportRepository
{
    IQueryable<SupportTicket> QueryTickets();
    IQueryable<SupportTicketMessage> QueryMessages();
    IQueryable<JobDispute> QueryDisputes();
    Task<SupportTicket?> GetTicketAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JobDispute?> GetDisputeAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddTicketAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
    Task AddMessageAsync(SupportTicketMessage message, CancellationToken cancellationToken = default);
    Task AddDisputeAsync(JobDispute dispute, CancellationToken cancellationToken = default);
    Task<string> GenerateTicketNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ISupportJobLookupService
{
    Task<SupportJobLookupResult?> GetSupportJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}
