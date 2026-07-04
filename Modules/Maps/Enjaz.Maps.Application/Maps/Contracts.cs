using Enjaz.Maps.Domain.Maps;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Maps.Application.Maps;

public interface IMapsService
{
    Task<Result<ServiceZoneCheckResponse>> CheckServiceZoneAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ServiceZoneSummaryResponse>>> GetActiveServiceZonesAsync(CancellationToken cancellationToken = default);
    Task<Result<ServiceZoneDetailsResponse>> GetServiceZoneDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ServiceZoneDetailsResponse>> CreateServiceZoneAsync(ServiceZoneRequest request, CancellationToken cancellationToken = default);
    Task<Result<ServiceZoneDetailsResponse>> UpdateServiceZoneAsync(Guid id, ServiceZoneRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetServiceZoneActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    Task<Result<UpdateTechnicianLocationResponse>> UpdateTechnicianLocationAsync(UpdateTechnicianLocationRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<NearbyTechnicianResponse>>> GetNearbyTechniciansAsync(decimal latitude, decimal longitude, Guid serviceId, int radiusMeters = 5000, CancellationToken cancellationToken = default);
}

public interface IMapsRepository
{
    Task<ServiceZone?> GetServiceZoneAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceZone?> GetServiceZoneBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<ServiceZone?> GetContainingActiveServiceZoneAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ServiceZone>> GetActiveServiceZonesAsync(CancellationToken cancellationToken = default);
    Task AddServiceZoneAsync(ServiceZone serviceZone, CancellationToken cancellationToken = default);
    Task<TechnicianLocationSnapshot?> GetTechnicianLocationSnapshotAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task AddTechnicianLocationSnapshotAsync(TechnicianLocationSnapshot snapshot, CancellationToken cancellationToken = default);
    Task AddTechnicianLocationHistoryAsync(TechnicianLocationHistory history, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<NearbyTechnicianResponse>> GetNearbyTechniciansAsync(decimal latitude, decimal longitude, Guid serviceId, int radiusMeters, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ITechnicianLocationLookupService
{
    Task<TechnicianLocationLookupResponse?> GetLatestLocationAsync(Guid technicianId, CancellationToken cancellationToken = default);
}

public interface IJobExecutionLookupService
{
    Task<TechnicianActiveJobLookupResult?> GetActiveJobForTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);
}

public interface ITechnicianLookupService
{
    Task<TechnicianLookupResult?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TechnicianLookupResult?> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<TechnicianPublicLookupResult?> GetPublicProfileAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<bool> IsApprovedAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<bool> IsOnlineAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<bool> HasSkillAsync(Guid technicianId, Guid serviceId, CancellationToken cancellationToken = default);
}

public interface ITrackingEventPublisher
{
    Task PublishTechnicianLocationUpdatedAsync(TechnicianLocationUpdatedEvent updatedEvent, CancellationToken cancellationToken = default);
}
