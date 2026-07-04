using Enjaz.Calls.Application.Calls;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Calls.Endpoints.Calls;

[ApiController]
public sealed class CallsController(ICallsService callsService) : ControllerBase
{
    [Authorize(Roles = "Customer,Technician")]
    [HttpPost("api/v1/jobs/{jobId:guid}/calls/start")]
    public async Task<IActionResult> Start(Guid jobId, CancellationToken cancellationToken) => ToActionResult(await callsService.StartCallAsync(jobId, cancellationToken));

    [Authorize(Roles = "Customer,Technician")]
    [HttpGet("api/v1/jobs/{jobId:guid}/calls")]
    public async Task<IActionResult> GetJobCalls(Guid jobId, CancellationToken cancellationToken) => ToActionResult(await callsService.GetJobCallsAsync(jobId, cancellationToken));

    [AllowAnonymous]
    [HttpPost("api/v1/calls/webhooks/callera")]
    public async Task<IActionResult> CalleraWebhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var raw = await reader.ReadToEndAsync(cancellationToken);
        var headers = Request.Headers.ToDictionary(header => header.Key, header => (string?)string.Join(",", header.Value.ToArray()), StringComparer.OrdinalIgnoreCase);
        return ToActionResult(await callsService.ProcessCalleraWebhookAsync(new CallWebhookProcessRequest(string.IsNullOrWhiteSpace(raw) ? "{}" : raw, headers), cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
