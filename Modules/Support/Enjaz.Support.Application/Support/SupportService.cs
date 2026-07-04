using Enjaz.Notifications.Application.Notifications;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Enjaz.Support.Domain.Support;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Support.Application.Support;

public sealed class SupportService(ISupportRepository repository, ISupportJobLookupService jobLookupService, ICurrentUserContext currentUserContext, INotificationService notificationService) : ISupportService
{
    private static readonly IReadOnlySet<string> DisputableStatuses = new HashSet<string> { "Paid", "SearchingTechnician", "WaitingForManualAssignment", "TechnicianAssigned", "TechnicianAccepted", "TechnicianOnWay", "Arrived", "InProgress", "Completed", "Disputed" };

    public async Task<Result<SupportTicketResponse>> CreateTicketAsync(CreateSupportTicketRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId == Guid.Empty) return Result.Failure<SupportTicketResponse>("unauthorized", "Authentication is required.");
        if (request.RelatedJobId.HasValue)
        {
            var related = await jobLookupService.GetSupportJobAsync(request.RelatedJobId.Value, cancellationToken);
            if (related is null) return Result.Failure<SupportTicketResponse>("job_not_found", "Related job was not found.");
            if (!IsParticipant(related, currentUserContext.UserId)) return Result.Failure<SupportTicketResponse>("forbidden", "You cannot create a ticket for this job.");
        }

        var now = DateTime.UtcNow;
        var ticket = new SupportTicket { TicketNumber = await repository.GenerateTicketNumberAsync(now, cancellationToken), CreatedByUserId = currentUserContext.UserId, RelatedJobId = request.RelatedJobId, Category = request.Category, Priority = request.Priority, Subject = request.Subject.Trim(), Description = request.Description.Trim(), CreatedAtUtc = now };
        await repository.AddTicketAsync(ticket, cancellationToken);
        await repository.AddMessageAsync(new SupportTicketMessage { TicketId = ticket.Id, SenderUserId = currentUserContext.UserId, SenderRole = SupportSenderRoles.Customer, Message = ticket.Description, CreatedAtUtc = now }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        if (ticket.Priority == SupportTicketPriorities.Urgent) await NotifyAdminAsync("support.ticket.urgent", "Urgent ticket created", ticket.Id, cancellationToken);
        return Result.Success(await MapTicketAsync(ticket, false, cancellationToken));
    }

    public async Task<Result<IReadOnlyCollection<SupportTicketResponse>>> GetMyTicketsAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await repository.QueryTickets().AsNoTracking().Where(ticket => ticket.CreatedByUserId == currentUserContext.UserId).OrderByDescending(ticket => ticket.CreatedAtUtc).Take(200).ToListAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<SupportTicketResponse>>(await MapTicketsAsync(tickets, false, cancellationToken));
    }

    public async Task<Result<SupportTicketResponse>> GetTicketAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetTicketAsync(id, cancellationToken);
        if (ticket is null) return Result.Failure<SupportTicketResponse>("ticket_not_found", "Ticket was not found.");
        if (ticket.CreatedByUserId != currentUserContext.UserId) return Result.Failure<SupportTicketResponse>("forbidden", "You cannot access this ticket.");
        return Result.Success(await MapTicketAsync(ticket, false, cancellationToken));
    }

    public async Task<Result<SupportTicketMessageResponse>> AddMessageAsync(Guid id, AddTicketMessageRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetTicketAsync(id, cancellationToken);
        if (ticket is null) return Result.Failure<SupportTicketMessageResponse>("ticket_not_found", "Ticket was not found.");
        if (ticket.CreatedByUserId != currentUserContext.UserId) return Result.Failure<SupportTicketMessageResponse>("forbidden", "You cannot update this ticket.");
        var message = new SupportTicketMessage { TicketId = id, SenderUserId = currentUserContext.UserId, SenderRole = SupportSenderRoles.Customer, Message = request.Message.Trim(), CreatedAtUtc = DateTime.UtcNow };
        await repository.AddMessageAsync(message, cancellationToken);
        ticket.UpdatedAtUtc = message.CreatedAtUtc;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapMessage(message));
    }

    public async Task<Result<JobDisputeResponse>> OpenDisputeAsync(Guid jobId, OpenDisputeRequest request, CancellationToken cancellationToken = default)
    {
        var job = await jobLookupService.GetSupportJobAsync(jobId, cancellationToken);
        if (job is null) return Result.Failure<JobDisputeResponse>("job_not_found", "Job was not found.");
        if (!IsParticipant(job, currentUserContext.UserId)) return Result.Failure<JobDisputeResponse>("forbidden", "Only job participants can open a dispute.");
        if (!DisputableStatuses.Contains(job.Status)) return Result.Failure<JobDisputeResponse>("job_not_disputable", "This job cannot be disputed yet.");
        var dispute = new JobDispute { JobId = jobId, OpenedByUserId = currentUserContext.UserId, Reason = request.Reason.Trim(), Status = JobDisputeStatuses.Open, CreatedAtUtc = DateTime.UtcNow };
        await repository.AddDisputeAsync(dispute, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await NotifyAdminAsync("support.dispute.opened", "Job dispute opened", dispute.Id, cancellationToken);
        return Result.Success(MapDispute(dispute));
    }

    public async Task<Result<IReadOnlyCollection<SupportTicketResponse>>> GetTicketsAsync(SupportTicketQuery query, CancellationToken cancellationToken = default)
    {
        var tickets = repository.QueryTickets();
        if (!string.IsNullOrWhiteSpace(query.Status)) tickets = tickets.Where(ticket => ticket.Status == query.Status);
        if (!string.IsNullOrWhiteSpace(query.Priority)) tickets = tickets.Where(ticket => ticket.Priority == query.Priority);
        if (query.RelatedJobId.HasValue) tickets = tickets.Where(ticket => ticket.RelatedJobId == query.RelatedJobId);
        return Result.Success<IReadOnlyCollection<SupportTicketResponse>>(await MapTicketsAsync(await tickets.AsNoTracking().OrderByDescending(ticket => ticket.CreatedAtUtc).Take(300).ToListAsync(cancellationToken), true, cancellationToken));
    }

    public async Task<Result<SupportTicketResponse>> GetTicketForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetTicketAsync(id, cancellationToken);
        return ticket is null ? Result.Failure<SupportTicketResponse>("ticket_not_found", "Ticket was not found.") : Result.Success(await MapTicketAsync(ticket, true, cancellationToken));
    }

    public async Task<Result<SupportTicketResponse>> AssignTicketAsync(Guid id, AdminAssignTicketRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetTicketAsync(id, cancellationToken);
        if (ticket is null) return Result.Failure<SupportTicketResponse>("ticket_not_found", "Ticket was not found.");
        ticket.AssignedAdminUserId = request.AdminUserId;
        ticket.Status = SupportTicketStatuses.InProgress;
        ticket.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(await MapTicketAsync(ticket, true, cancellationToken));
    }

    public async Task<Result<SupportTicketResponse>> UpdateTicketStatusAsync(Guid id, AdminUpdateTicketStatusRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetTicketAsync(id, cancellationToken);
        if (ticket is null) return Result.Failure<SupportTicketResponse>("ticket_not_found", "Ticket was not found.");
        ticket.Status = request.Status;
        ticket.UpdatedAtUtc = DateTime.UtcNow;
        ticket.ClosedAtUtc = request.Status is SupportTicketStatuses.Closed or SupportTicketStatuses.Resolved ? ticket.UpdatedAtUtc : null;
        await repository.SaveChangesAsync(cancellationToken);
        await notificationService.SendInAppAsync(ticket.CreatedByUserId, "support.ticket.status", "Ticket status updated", $"Your support ticket is now {ticket.Status}.", new Dictionary<string, string?> { ["ticketId"] = ticket.Id.ToString(), ["status"] = ticket.Status }, cancellationToken);
        return Result.Success(await MapTicketAsync(ticket, true, cancellationToken));
    }

    public async Task<Result<SupportTicketMessageResponse>> AddAdminMessageAsync(Guid id, AddTicketMessageRequest request, CancellationToken cancellationToken = default)
    {
        var ticket = await repository.GetTicketAsync(id, cancellationToken);
        if (ticket is null) return Result.Failure<SupportTicketMessageResponse>("ticket_not_found", "Ticket was not found.");
        var message = new SupportTicketMessage { TicketId = id, SenderUserId = currentUserContext.UserId, SenderRole = SupportSenderRoles.Admin, Message = request.Message.Trim(), CreatedAtUtc = DateTime.UtcNow };
        await repository.AddMessageAsync(message, cancellationToken);
        ticket.UpdatedAtUtc = message.CreatedAtUtc;
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success(MapMessage(message));
    }

    public async Task<Result<IReadOnlyCollection<JobDisputeResponse>>> GetDisputesAsync(JobDisputeQuery query, CancellationToken cancellationToken = default)
    {
        var disputes = repository.QueryDisputes();
        if (!string.IsNullOrWhiteSpace(query.Status)) disputes = disputes.Where(dispute => dispute.Status == query.Status);
        if (query.JobId.HasValue) disputes = disputes.Where(dispute => dispute.JobId == query.JobId);
        return Result.Success<IReadOnlyCollection<JobDisputeResponse>>(await disputes.AsNoTracking().OrderByDescending(dispute => dispute.CreatedAtUtc).Take(300).Select(dispute => MapDispute(dispute)).ToListAsync(cancellationToken));
    }

    public Task<Result<JobDisputeResponse>> ResolveDisputeAsync(Guid id, ResolveDisputeRequest request, CancellationToken cancellationToken = default) => CloseDisputeAsync(id, request.Resolution, JobDisputeStatuses.Resolved, cancellationToken);
    public Task<Result<JobDisputeResponse>> RejectDisputeAsync(Guid id, ResolveDisputeRequest request, CancellationToken cancellationToken = default) => CloseDisputeAsync(id, request.Resolution, JobDisputeStatuses.Rejected, cancellationToken);

    private async Task<Result<JobDisputeResponse>> CloseDisputeAsync(Guid id, string resolution, string status, CancellationToken cancellationToken)
    {
        var dispute = await repository.GetDisputeAsync(id, cancellationToken);
        if (dispute is null) return Result.Failure<JobDisputeResponse>("dispute_not_found", "Dispute was not found.");
        dispute.Status = status;
        dispute.Resolution = resolution.Trim();
        dispute.ResolvedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);
        await notificationService.SendInAppAsync(dispute.OpenedByUserId, "support.dispute.closed", "Dispute updated", $"Your dispute was {status.ToLowerInvariant()}.", new Dictionary<string, string?> { ["disputeId"] = dispute.Id.ToString(), ["status"] = status }, cancellationToken);
        return Result.Success(MapDispute(dispute));
    }

    private async Task<IReadOnlyCollection<SupportTicketResponse>> MapTicketsAsync(IReadOnlyCollection<SupportTicket> tickets, bool includeInternal, CancellationToken cancellationToken)
    {
        var result = new List<SupportTicketResponse>();
        foreach (var ticket in tickets) result.Add(await MapTicketAsync(ticket, includeInternal, cancellationToken));
        return result;
    }

    private async Task<SupportTicketResponse> MapTicketAsync(SupportTicket ticket, bool includeInternal, CancellationToken cancellationToken)
    {
        var messages = await repository.QueryMessages().AsNoTracking().Where(message => message.TicketId == ticket.Id && (includeInternal || !message.IsInternal)).OrderBy(message => message.CreatedAtUtc).Select(message => MapMessage(message)).ToListAsync(cancellationToken);
        return new SupportTicketResponse(ticket.Id, ticket.TicketNumber, ticket.CreatedByUserId, ticket.RelatedJobId, ticket.Category, ticket.Priority, ticket.Status, ticket.Subject, ticket.Description, ticket.AssignedAdminUserId, ticket.CreatedAtUtc, ticket.UpdatedAtUtc, ticket.ClosedAtUtc, messages);
    }

    private static SupportTicketMessageResponse MapMessage(SupportTicketMessage message) => new(message.Id, message.TicketId, message.SenderUserId, message.SenderRole, message.Message, message.IsInternal, message.CreatedAtUtc);
    private static JobDisputeResponse MapDispute(JobDispute dispute) => new(dispute.Id, dispute.JobId, dispute.OpenedByUserId, dispute.Reason, dispute.Status, dispute.Resolution, dispute.CreatedAtUtc, dispute.ResolvedAtUtc);
    private static bool IsParticipant(SupportJobLookupResult job, Guid userId) => job.CustomerUserId == userId || job.AssignedTechnicianUserId == userId;
    private Task NotifyAdminAsync(string type, string title, Guid id, CancellationToken cancellationToken) => notificationService.SendInAppAsync(Guid.Empty, type, title, "Admin support attention is required.", new Dictionary<string, string?> { ["id"] = id.ToString() }, cancellationToken);
}
