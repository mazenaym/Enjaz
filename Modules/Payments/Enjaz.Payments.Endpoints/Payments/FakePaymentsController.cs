using Enjaz.Payments.Application.Payments;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Enjaz.Payments.Endpoints.Payments;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/payments/fake")]
public sealed class FakePaymentsController(IPaymentsService paymentsService, IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("{paymentId:guid}/succeed")]
    public async Task<IActionResult> Succeed(Guid paymentId, CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment()) return NotFound();
        return ToActionResult(await paymentsService.SimulateFakeSuccessAsync(paymentId, cancellationToken));
    }

    [HttpPost("{paymentId:guid}/fail")]
    public async Task<IActionResult> Fail(Guid paymentId, FakePaymentFailRequest request, CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment()) return NotFound();
        return ToActionResult(await paymentsService.SimulateFakeFailureAsync(paymentId, request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
