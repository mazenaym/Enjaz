using NetTopologySuite.Geometries;

namespace Enjaz.Maps.Domain.Maps;

public sealed class TechnicianLocationSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TechnicianId { get; set; }
    public Guid UserId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public Point Location { get; set; } = default!;
    public decimal? AccuracyMeters { get; set; }
    public decimal? Heading { get; set; }
    public decimal? SpeedMetersPerSecond { get; set; }
    public string? Source { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
