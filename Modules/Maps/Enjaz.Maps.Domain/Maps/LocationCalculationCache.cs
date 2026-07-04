namespace Enjaz.Maps.Domain.Maps;

public sealed class LocationCalculationCache
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal OriginLatitude { get; set; }
    public decimal OriginLongitude { get; set; }
    public decimal DestinationLatitude { get; set; }
    public decimal DestinationLongitude { get; set; }
    public decimal? DistanceMeters { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Provider { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
}
