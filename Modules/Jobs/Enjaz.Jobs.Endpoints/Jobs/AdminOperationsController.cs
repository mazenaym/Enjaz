using Enjaz.Jobs.Application.Jobs;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Jobs.Endpoints.Jobs;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/operations/jobs")]
public sealed class AdminOperationsController(IAdminOperationsService operationsService) : ControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveJobs(CancellationToken cancellationToken)
    {
        return ToActionResult(await operationsService.GetActiveJobsAsync(cancellationToken));
    }

    [HttpGet("{jobId:guid}")]
    public async Task<IActionResult> GetDetails(Guid jobId, CancellationToken cancellationToken)
    {
        return ToActionResult(await operationsService.GetJobOperationsDetailsAsync(jobId, cancellationToken));
    }

    [HttpPost("{jobId:guid}/force-complete")]
    public async Task<IActionResult> ForceComplete(Guid jobId, AdminForceCompleteRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await operationsService.ForceCompleteAsync(jobId, request, cancellationToken));
    }

    [HttpPost("{jobId:guid}/dispute")]
    public async Task<IActionResult> Dispute(Guid jobId, DisputeRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await operationsService.MarkDisputedAsync(jobId, request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
