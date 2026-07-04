namespace Enjaz.Jobs.Domain.Jobs;

public sealed class JobCounter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string YearMonth { get; set; } = string.Empty;
    public int LastNumber { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
