using Enjaz.Maps.Application.Maps;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Maps.Endpoints.Maps;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/maps")]
public sealed class AdminMapsController(IMapsService mapsService) : ControllerBase
{
    [HttpPost("service-zones")]
    public async Task<IActionResult> CreateServiceZone(ServiceZoneRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.CreateServiceZoneAsync(request, cancellationToken));
    }

    [HttpPut("service-zones/{id:guid}")]
    public async Task<IActionResult> UpdateServiceZone(Guid id, ServiceZoneRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.UpdateServiceZoneAsync(id, request, cancellationToken));
    }

    [HttpPost("service-zones/{id:guid}/activate")]
    public async Task<IActionResult> ActivateServiceZone(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.SetServiceZoneActiveAsync(id, true, cancellationToken));
    }

    [HttpPost("service-zones/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateServiceZone(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await mapsService.SetServiceZoneActiveAsync(id, false, cancellationToken));
    }

    private static IActionResult ToActionResult(Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(new { message = "Success" })
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
