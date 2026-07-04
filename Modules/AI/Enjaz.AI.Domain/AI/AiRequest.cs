namespace Enjaz.AI.Domain.AI;

public sealed class AiRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public string Feature { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string InputText { get; set; } = string.Empty;
    public string? InputJson { get; set; }
    public string? RawResponseJson { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
