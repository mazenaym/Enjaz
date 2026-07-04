namespace Enjaz.Technicians.Domain.Technicians;

public sealed class TechnicianDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TechnicianId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileKey { get; set; }
    public string Status { get; set; } = TechnicianDocumentStatuses.Pending;
    public string? RejectionReason { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAtUtc { get; set; }
    public TechnicianProfile? Technician { get; set; }
}
