using Enjaz.Maps.Application.Maps;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Maps.Endpoints.Technicians;

[ApiController]
[Route("api/v1/technicians")]
public sealed class TechnicianLocationController(IMapsService mapsService) : ControllerBase
{
    [HttpPost("location")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> UpdateLocation(UpdateTechnicianLocationRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.UpdateTechnicianLocationAsync(request, cancellationToken));
    }

    [HttpGet("nearby")]
    [Authorize]
    public async Task<IActionResult> GetNearbyTechnicians(
        [FromQuery] decimal lat,
        [FromQuery] decimal lng,
        [FromQuery] Guid serviceId,
        [FromQuery] int radiusMeters,
        CancellationToken cancellationToken)
    {
        var radius = radiusMeters <= 0 ? 5000 : radiusMeters;
        return ToActionResult(await mapsService.GetNearbyTechniciansAsync(lat, lng, serviceId, radius, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
