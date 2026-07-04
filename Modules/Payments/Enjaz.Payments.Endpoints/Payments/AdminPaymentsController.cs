using Enjaz.Payments.Application.Payments;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Payments.Endpoints.Payments;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/payments")]
public sealed class AdminPaymentsController(IAdminPaymentsService paymentsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPayments([FromQuery] string? status, [FromQuery] Guid? jobId, [FromQuery] Guid? customerUserId, CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetPaymentsAsync(status, jobId, customerUserId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetPaymentAsync(id, cancellationToken));
    }

    [HttpGet("job/{jobId:guid}")]
    public async Task<IActionResult> GetPaymentsForJob(Guid jobId, CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetPaymentsForJobAsync(jobId, cancellationToken));
    }

    [HttpGet("webhooks")]
    public async Task<IActionResult> GetWebhookLogs(CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetWebhookLogsAsync(cancellationToken));
    }

    [HttpGet("webhooks/{id:guid}")]
    public async Task<IActionResult> GetWebhookLog(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await paymentsService.GetWebhookLogAsync(id, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
