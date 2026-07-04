using Enjaz.Jobs.Application.Jobs;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Jobs.Endpoints.Jobs;

[ApiController]
[Authorize(Roles = "Technician")]
[Route("api/v1/technicians/jobs")]
public sealed class TechnicianJobsController(ITechnicianJobsService jobsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAssignedJobs(CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.GetMyAssignedJobsAsync(cancellationToken));
    }

    [HttpPost("{jobId:guid}/accept")]
    public async Task<IActionResult> Accept(Guid jobId, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.AcceptAssignmentAsync(jobId, cancellationToken));
    }

    [HttpPost("{jobId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid jobId, RejectAssignmentRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.RejectAssignmentAsync(jobId, request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
