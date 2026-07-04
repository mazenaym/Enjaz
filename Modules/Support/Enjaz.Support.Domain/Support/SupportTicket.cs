namespace Enjaz.Support.Domain.Support;

public sealed class SupportTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TicketNumber { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public Guid? RelatedJobId { get; set; }
    public string Category { get; set; } = SupportTicketCategories.General;
    public string Priority { get; set; } = SupportTicketPriorities.Normal;
    public string Status { get; set; } = SupportTicketStatuses.Open;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? AssignedAdminUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
}
