using Enjaz.Jobs.Application.Jobs;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Jobs.Endpoints.Jobs;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/v1/jobs")]
public sealed class JobsController(ICustomerJobsService jobsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateJobRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.CreateAsync(request, cancellationToken));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyJobs([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return ToActionResult(await jobsService.GetMyJobsAsync(new JobListQuery(status, page, pageSize), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMyJobDetails(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.GetMyJobDetailsAsync(id, cancellationToken));
    }

    [HttpGet("{id:guid}/tracking")]
    public async Task<IActionResult> GetTracking(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.GetTrackingAsync(id, cancellationToken));
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.GetTimelineAsync(id, cancellationToken));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancelJobRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.CancelAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/media")]
    public async Task<IActionResult> AddMedia(Guid id, JobMediaRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.AddMediaAsync(id, request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
