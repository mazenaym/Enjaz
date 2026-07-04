using NetTopologySuite.Geometries;

namespace Enjaz.Maps.Domain.Maps;

public sealed class ServiceZone
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string Slug { get; set; } = string.Empty;
    public Polygon Polygon { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
