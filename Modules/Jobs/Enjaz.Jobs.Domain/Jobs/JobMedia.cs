namespace Enjaz.Jobs.Domain.Jobs;

public sealed class JobMedia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public string MediaType { get; set; } = JobMediaTypes.Image;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileKey { get; set; }
    public string? Caption { get; set; }
    public Guid UploadedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
