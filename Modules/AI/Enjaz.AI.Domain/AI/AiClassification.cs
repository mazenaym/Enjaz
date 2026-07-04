namespace Enjaz.AI.Domain.AI;

public sealed class AiClassification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AiRequestId { get; set; }
    public Guid? UserId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public string CustomerDescription { get; set; } = string.Empty;
    public int ComplexityId { get; set; }
    public string ComplexityName { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public string? SuggestedAction { get; set; }
    public bool RequiresInspection { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
