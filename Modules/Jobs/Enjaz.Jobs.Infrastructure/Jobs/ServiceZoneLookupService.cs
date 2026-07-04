using Enjaz.Jobs.Application.Jobs;
using Enjaz.Maps.Application.Maps;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class ServiceZoneLookupService(IMapsRepository mapsRepository) : IServiceZoneLookupService
{
    public async Task<ServiceZoneCoverageResult> CheckLocationCoverageAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken = default)
    {
        var zone = await mapsRepository.GetContainingActiveServiceZoneAsync(latitude, longitude, cancellationToken);
        return new ServiceZoneCoverageResult(zone is not null, zone?.Id);
    }
}
