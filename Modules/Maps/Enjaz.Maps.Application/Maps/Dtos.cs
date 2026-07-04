namespace Enjaz.Maps.Application.Maps;

public sealed record LocationPointRequest(decimal Lat, decimal Lng);

public sealed record ServiceZoneRequest(
    string NameAr,
    string? NameEn,
    string City,
    string? Area,
    string Slug,
    IReadOnlyCollection<LocationPointRequest>? Polygon);

public sealed record ServiceZoneSummaryResponse(Guid Id, string NameAr, string? NameEn, string City, string? Area, string Slug);

public sealed record ServiceZoneDetailsResponse(
    Guid Id,
    string NameAr,
    string? NameEn,
    string City,
    string? Area,
    string Slug,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    IReadOnlyCollection<LocationPointResponse> Polygon);

public sealed record LocationPointResponse(decimal Lat, decimal Lng);

public sealed record ServiceZoneCheckResponse(bool IsCovered, ServiceZoneCheckZoneResponse? Zone);

public sealed record ServiceZoneCheckZoneResponse(Guid Id, string NameAr, string? NameEn, string City);

public sealed record UpdateTechnicianLocationRequest(
    decimal Latitude,
    decimal Longitude,
    decimal? AccuracyMeters,
    decimal? Heading,
    decimal? SpeedMetersPerSecond,
    string? Source);

public sealed record UpdateTechnicianLocationResponse(string Message, DateTime UpdatedAtUtc);

public sealed record NearbyTechnicianResponse(
    Guid TechnicianId,
    string FullName,
    string? ProfileImageUrl,
    decimal AverageRating,
    int TotalReviews,
    decimal DistanceMeters,
    string AvailabilityStatus,
    string? SkillLevel);

public sealed record TechnicianLookupResult(
    Guid TechnicianId,
    Guid UserId,
    string Status,
    string AvailabilityStatus);

public sealed record TechnicianLocationUpdatedEvent(
    Guid TechnicianId,
    decimal Latitude,
    decimal Longitude,
    DateTime UpdatedAtUtc);
