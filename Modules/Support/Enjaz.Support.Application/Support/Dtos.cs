namespace Enjaz.Support.Application.Support;

public sealed record CreateSupportTicketRequest(Guid? RelatedJobId, string Category, string Priority, string Subject, string Description);
public sealed record AddTicketMessageRequest(string Message);
public sealed record AdminAssignTicketRequest(Guid AdminUserId);
public sealed record AdminUpdateTicketStatusRequest(string Status);
public sealed record OpenDisputeRequest(string Reason);
public sealed record ResolveDisputeRequest(string Resolution);
public sealed record SupportTicketResponse(Guid Id, string TicketNumber, Guid CreatedByUserId, Guid? RelatedJobId, string Category, string Priority, string Status, string Subject, string Description, Guid? AssignedAdminUserId, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, DateTime? ClosedAtUtc, IReadOnlyCollection<SupportTicketMessageResponse> Messages);
public sealed record SupportTicketMessageResponse(Guid Id, Guid TicketId, Guid SenderUserId, string SenderRole, string Message, bool IsInternal, DateTime CreatedAtUtc);
public sealed record JobDisputeResponse(Guid Id, Guid JobId, Guid OpenedByUserId, string Reason, string Status, string? Resolution, DateTime CreatedAtUtc, DateTime? ResolvedAtUtc);
public sealed record SupportTicketQuery(string? Status, string? Priority, Guid? RelatedJobId);
public sealed record JobDisputeQuery(string? Status, Guid? JobId);
public sealed record SupportJobLookupResult(Guid JobId, Guid CustomerUserId, Guid? AssignedTechnicianUserId, string Status);
