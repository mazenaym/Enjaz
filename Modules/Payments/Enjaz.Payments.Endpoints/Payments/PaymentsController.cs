using Enjaz.Payments.Application.Payments;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Payments.Endpoints.Payments;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/v1/payments")]
public sealed class PaymentsController(IPaymentsService paymentsService) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout(CreateCheckoutRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.CreateCheckoutAsync(request, cancellationToken));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayments(CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetMyPaymentsAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetMyPaymentAsync(id, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
