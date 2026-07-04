using Enjaz.Catalog.Application.Catalog;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Catalog.Endpoints.Catalog;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/catalog")]
public sealed class AdminCatalogController(ICatalogService catalogService) : ControllerBase
{
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(CategoryRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.CreateCategoryAsync(request, cancellationToken));
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, CategoryRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.UpdateCategoryAsync(id, request, cancellationToken));
    }

    [HttpPost("categories/{id:guid}/activate")]
    public async Task<IActionResult> ActivateCategory(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.SetCategoryActiveAsync(id, true, cancellationToken));
    }

    [HttpPost("categories/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateCategory(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.SetCategoryActiveAsync(id, false, cancellationToken));
    }

    [HttpPost("services")]
    public async Task<IActionResult> CreateService(ServiceRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.CreateServiceAsync(request, cancellationToken));
    }

    [HttpPut("services/{id:guid}")]
    public async Task<IActionResult> UpdateService(Guid id, ServiceRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.UpdateServiceAsync(id, request, cancellationToken));
    }

    [HttpPost("services/{id:guid}/activate")]
    public async Task<IActionResult> ActivateService(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.SetServiceActiveAsync(id, true, cancellationToken));
    }

    [HttpPost("services/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateService(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.SetServiceActiveAsync(id, false, cancellationToken));
    }

    [HttpPost("service-tiers")]
    public async Task<IActionResult> CreateServiceTier(ServiceTierRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.CreateServiceTierAsync(request, cancellationToken));
    }

    [HttpPut("service-tiers/{id:guid}")]
    public async Task<IActionResult> UpdateServiceTier(Guid id, ServiceTierRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.UpdateServiceTierAsync(id, request, cancellationToken));
    }

    [HttpPost("service-tiers/{id:guid}/activate")]
    public async Task<IActionResult> ActivateServiceTier(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.SetServiceTierActiveAsync(id, true, cancellationToken));
    }

    [HttpPost("service-tiers/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateServiceTier(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await catalogService.SetServiceTierActiveAsync(id, false, cancellationToken));
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
