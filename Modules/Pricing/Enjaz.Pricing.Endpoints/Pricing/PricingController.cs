using Enjaz.Pricing.Application.Pricing;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Pricing.Endpoints.Pricing;

[ApiController]
[Route("api/v1/pricing")]
public sealed class PricingController(IPricingCalculator calculator) : ControllerBase
{
    [HttpPost("calculate")]
    [Authorize]
    public async Task<IActionResult> Calculate(CalculatePriceRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await calculator.CalculateAsync(request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
