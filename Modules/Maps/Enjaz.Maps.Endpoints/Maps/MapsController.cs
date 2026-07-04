using Enjaz.Maps.Application.Maps;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Maps.Endpoints.Maps;

[ApiController]
[Route("api/v1/maps")]
public sealed class MapsController(IMapsService mapsService) : ControllerBase
{
    [HttpGet("service-zones/check")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckServiceZone([FromQuery] decimal lat, [FromQuery] decimal lng, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.CheckServiceZoneAsync(lat, lng, cancellationToken));
    }

    [HttpGet("service-zones")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveServiceZones(CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.GetActiveServiceZonesAsync(cancellationToken));
    }

    [HttpGet("service-zones/{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServiceZoneDetails(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.GetServiceZoneDetailsAsync(id, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
