using Enjaz.Calls.Application.Calls;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Calls.Endpoints.Calls;

[ApiController]
[Authorize(Roles = "Admin")]
public sealed class AdminCallsController(ICallsService callsService) : ControllerBase
{
    [HttpGet("api/v1/admin/calls")]
    public async Task<IActionResult> GetCalls(CancellationToken cancellationToken) => ToActionResult(await callsService.GetCallsAsync(cancellationToken));

    [HttpGet("api/v1/admin/calls/{id:guid}")]
    public async Task<IActionResult> GetCall(Guid id, CancellationToken cancellationToken) => ToActionResult(await callsService.GetCallAsync(id, cancellationToken));

    [HttpGet("api/v1/admin/calls/webhooks")]
    public async Task<IActionResult> GetWebhooks(CancellationToken cancellationToken) => ToActionResult(await callsService.GetWebhookLogsAsync(cancellationToken));

    [HttpGet("api/v1/admin/calls/webhooks/{id:guid}")]
    public async Task<IActionResult> GetWebhook(Guid id, CancellationToken cancellationToken) => ToActionResult(await callsService.GetWebhookLogAsync(id, cancellationToken));

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
