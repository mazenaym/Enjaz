namespace Enjaz.Support.Domain.Support;

public sealed class SupportTicketMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public Guid SenderUserId { get; set; }
    public string SenderRole { get; set; } = SupportSenderRoles.Customer;
    public string Message { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
