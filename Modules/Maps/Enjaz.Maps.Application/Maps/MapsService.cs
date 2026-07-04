using System.Text.Json;
using Enjaz.Maps.Domain.Maps;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.Extensions.Caching.Distributed;
using NetTopologySuite.Geometries;

namespace Enjaz.Maps.Application.Maps;

public sealed class MapsService(
    IMapsRepository repository,
    ICurrentUserContext currentUserContext,
    ITechnicianLookupService technicianLookupService,
    IJobExecutionLookupService jobExecutionLookupService,
    IDistributedCache distributedCache,
    ITrackingEventPublisher trackingEventPublisher) : IMapsService
{
    private const int MaxRadiusMeters = 20000;
    private const int DefaultRadiusMeters = 5000;
    private const string ApprovedStatus = "Approved";
    private const string OnlineAvailability = "Online";

    public async Task<Result<ServiceZoneCheckResponse>> CheckServiceZoneAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        if (!CoordinatesAreValid(latitude, longitude))
        {
            return Result.Failure<ServiceZoneCheckResponse>("invalid_coordinates", "Latitude or longitude is invalid.");
        }

        var zone = await repository.GetContainingActiveServiceZoneAsync(latitude, longitude, cancellationToken);
        if (zone is null)
        {
            return Result.Success(new ServiceZoneCheckResponse(false, null));
        }

        return Result.Success(new ServiceZoneCheckResponse(true, new ServiceZoneCheckZoneResponse(zone.Id, zone.NameAr, zone.NameEn, zone.City)));
    }

    public async Task<Result<IReadOnlyCollection<ServiceZoneSummaryResponse>>> GetActiveServiceZonesAsync(CancellationToken cancellationToken = default)
    {
        var zones = await repository.GetActiveServiceZonesAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<ServiceZoneSummaryResponse>>(zones.Select(MapSummary).ToArray());
    }

    public async Task<Result<ServiceZoneDetailsResponse>> GetServiceZoneDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await repository.GetServiceZoneAsync(id, cancellationToken);
        return zone is null
            ? Result.Failure<ServiceZoneDetailsResponse>("service_zone_not_found", "Service zone was not found.")
            : Result.Success(MapDetails(zone));
    }

    public async Task<Result<ServiceZoneDetailsResponse>> CreateServiceZoneAsync(ServiceZoneRequest request, CancellationToken cancellationToken = default)
    {
        var slug = request.Slug.Trim();
        if (await repository.GetServiceZoneBySlugAsync(slug, cancellationToken) is not null)
        {
            return Result.Failure<ServiceZoneDetailsResponse>("service_zone_slug_exists", "Service zone slug already exists.");
        }

        var polygonResult = CreatePolygon(request.Polygon);
        if (polygonResult.IsFailure)
        {
            return Result.Failure<ServiceZoneDetailsResponse>(polygonResult.ErrorCode!, polygonResult.ErrorMessage!);
        }

        var zone = new ServiceZone
        {
            NameAr = request.NameAr.Trim(),
            NameEn = TrimOptional(request.NameEn),
            City = request.City.Trim(),
            Area = TrimOptional(request.Area),
            Slug = slug,
            Polygon = polygonResult.Value!,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddServiceZoneAsync(zone, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapDetails(zone));
    }

    public async Task<Result<ServiceZoneDetailsResponse>> UpdateServiceZoneAsync(Guid id, ServiceZoneRequest request, CancellationToken cancellationToken = default)
    {
        var zone = await repository.GetServiceZoneAsync(id, cancellationToken);
        if (zone is null)
        {
            return Result.Failure<ServiceZoneDetailsResponse>("service_zone_not_found", "Service zone was not found.");
        }

        var slug = request.Slug.Trim();
        var existingSlugZone = await repository.GetServiceZoneBySlugAsync(slug, cancellationToken);
        if (existingSlugZone is not null && existingSlugZone.Id != id)
        {
            return Result.Failure<ServiceZoneDetailsResponse>("service_zone_slug_exists", "Service zone slug already exists.");
        }

        zone.NameAr = request.NameAr.Trim();
        zone.NameEn = TrimOptional(request.NameEn);
        zone.City = request.City.Trim();
        zone.Area = TrimOptional(request.Area);
        zone.Slug = slug;
        if (request.Polygon is not null && request.Polygon.Count > 0)
        {
            var polygonResult = CreatePolygon(request.Polygon);
            if (polygonResult.IsFailure)
            {
                return Result.Failure<ServiceZoneDetailsResponse>(polygonResult.ErrorCode!, polygonResult.ErrorMessage!);
            }

            zone.Polygon = polygonResult.Value!;
        }

        zone.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(MapDetails(zone));
    }

    public async Task<Result> SetServiceZoneActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var zone = await repository.GetServiceZoneAsync(id, cancellationToken);
        if (zone is null)
        {
            return Result.Failure("service_zone_not_found", "Service zone was not found.");
        }

        zone.IsActive = isActive;
        zone.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<UpdateTechnicianLocationResponse>> UpdateTechnicianLocationAsync(UpdateTechnicianLocationRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId == Guid.Empty)
        {
            return Result.Failure<UpdateTechnicianLocationResponse>("unauthenticated", "Authentication is required.");
        }

        var technician = await technicianLookupService.GetByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (technician is null)
        {
            return Result.Failure<UpdateTechnicianLocationResponse>("technician_profile_not_found", "Technician profile was not found.");
        }

        if (!string.Equals(technician.Status, ApprovedStatus, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<UpdateTechnicianLocationResponse>("technician_not_approved", "Only approved technicians can update location.");
        }

        if (!string.Equals(technician.AvailabilityStatus, OnlineAvailability, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<UpdateTechnicianLocationResponse>("technician_offline", "Only online technicians can update location.");
        }

        var now = DateTime.UtcNow;
        var location = CreatePoint(request.Latitude, request.Longitude);
        var snapshot = await repository.GetTechnicianLocationSnapshotAsync(technician.TechnicianId, cancellationToken);
        if (snapshot is null)
        {
            snapshot = new TechnicianLocationSnapshot { TechnicianId = technician.TechnicianId, UserId = technician.UserId };
            await repository.AddTechnicianLocationSnapshotAsync(snapshot, cancellationToken);
        }

        ApplyLocation(snapshot, request, location, now);
        await repository.AddTechnicianLocationHistoryAsync(new TechnicianLocationHistory
        {
            TechnicianId = technician.TechnicianId,
            UserId = technician.UserId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Location = location,
            AccuracyMeters = request.AccuracyMeters,
            Heading = request.Heading,
            SpeedMetersPerSecond = request.SpeedMetersPerSecond,
            Source = TrimOptional(request.Source),
            CreatedAtUtc = now
        }, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);
        await CacheLiveLocationAsync(technician, request, now, cancellationToken);
        var activeJob = await jobExecutionLookupService.GetActiveJobForTechnicianAsync(technician.TechnicianId, cancellationToken);
        await trackingEventPublisher.PublishTechnicianLocationUpdatedAsync(
            new TechnicianLocationUpdatedEvent(technician.TechnicianId, request.Latitude, request.Longitude, now, activeJob?.JobId, activeJob?.CustomerUserId, activeJob?.TechnicianUserId),
            cancellationToken);

        return Result.Success(new UpdateTechnicianLocationResponse("Location updated successfully", now));
    }

    public async Task<Result<IReadOnlyCollection<NearbyTechnicianResponse>>> GetNearbyTechniciansAsync(decimal latitude, decimal longitude, Guid serviceId, int radiusMeters = DefaultRadiusMeters, CancellationToken cancellationToken = default)
    {
        if (!CoordinatesAreValid(latitude, longitude))
        {
            return Result.Failure<IReadOnlyCollection<NearbyTechnicianResponse>>("invalid_coordinates", "Latitude or longitude is invalid.");
        }

        if (serviceId == Guid.Empty)
        {
            return Result.Failure<IReadOnlyCollection<NearbyTechnicianResponse>>("invalid_service", "Service id is required.");
        }

        if (radiusMeters is <= 0 or > MaxRadiusMeters)
        {
            return Result.Failure<IReadOnlyCollection<NearbyTechnicianResponse>>("invalid_radius", $"Radius must be between 1 and {MaxRadiusMeters} meters.");
        }

        var technicians = await repository.GetNearbyTechniciansAsync(latitude, longitude, serviceId, radiusMeters, cancellationToken);
        return Result.Success(technicians);
    }

    private static void ApplyLocation(TechnicianLocationSnapshot snapshot, UpdateTechnicianLocationRequest request, Point location, DateTime now)
    {
        snapshot.Latitude = request.Latitude;
        snapshot.Longitude = request.Longitude;
        snapshot.Location = location;
        snapshot.AccuracyMeters = request.AccuracyMeters;
        snapshot.Heading = request.Heading;
        snapshot.SpeedMetersPerSecond = request.SpeedMetersPerSecond;
        snapshot.Source = TrimOptional(request.Source);
        snapshot.UpdatedAtUtc = now;
    }

    private async Task CacheLiveLocationAsync(TechnicianLookupResult technician, UpdateTechnicianLocationRequest request, DateTime updatedAtUtc, CancellationToken cancellationToken)
    {
        var cacheValue = JsonSerializer.Serialize(new
        {
            technicianId = technician.TechnicianId,
            userId = technician.UserId,
            latitude = request.Latitude,
            longitude = request.Longitude,
            updatedAtUtc
        });

        await distributedCache.SetStringAsync(
            $"technician:location:{technician.TechnicianId}",
            cacheValue,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) },
            cancellationToken);
    }

    private static ServiceZoneSummaryResponse MapSummary(ServiceZone zone)
    {
        return new ServiceZoneSummaryResponse(zone.Id, zone.NameAr, zone.NameEn, zone.City, zone.Area, zone.Slug);
    }

    private static ServiceZoneDetailsResponse MapDetails(ServiceZone zone)
    {
        return new ServiceZoneDetailsResponse(
            zone.Id,
            zone.NameAr,
            zone.NameEn,
            zone.City,
            zone.Area,
            zone.Slug,
            zone.IsActive,
            zone.CreatedAtUtc,
            zone.UpdatedAtUtc,
            zone.Polygon.Coordinates.Select(coordinate => new LocationPointResponse((decimal)coordinate.Y, (decimal)coordinate.X)).ToArray());
    }

    private static Result<Polygon> CreatePolygon(IReadOnlyCollection<LocationPointRequest>? points)
    {
        if (points is null || points.Select(point => (point.Lat, point.Lng)).Distinct().Count() < 3)
        {
            return Result.Failure<Polygon>("invalid_polygon", "Polygon must contain at least 3 unique points.");
        }

        var coordinates = points.Select(point => new Coordinate((double)point.Lng, (double)point.Lat)).ToList();
        if (!coordinates[0].Equals2D(coordinates[^1]))
        {
            coordinates.Add(coordinates[0]);
        }

        return Result.Success(new Polygon(new LinearRing(coordinates.ToArray())) { SRID = 4326 });
    }

    private static Point CreatePoint(decimal latitude, decimal longitude)
    {
        return new Point((double)longitude, (double)latitude) { SRID = 4326 };
    }

    private static bool CoordinatesAreValid(decimal latitude, decimal longitude)
    {
        return latitude is >= -90m and <= 90m && longitude is >= -180m and <= 180m;
    }

    private static string? TrimOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
