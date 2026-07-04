using Enjaz.Catalog.Application.Catalog;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Catalog.Endpoints.Catalog;

[ApiController]
[Route("api/v1/catalog")]
public sealed class CatalogController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.GetActiveCategoriesAsync(cancellationToken));
    }

    [HttpGet("categories/{categoryId:guid}/services")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicesByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.GetServicesByCategoryAsync(categoryId, cancellationToken));
    }

    [HttpGet("services")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServices(CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.GetActiveServicesAsync(cancellationToken));
    }

    [HttpGet("services/{serviceId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetService(Guid serviceId, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.GetServiceDetailsAsync(serviceId, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
