using Enjaz.Jobs.Application.Jobs;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Jobs.Endpoints.Jobs;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/jobs")]
public sealed class AdminJobsController(IAdminJobsService jobsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJobs(
        [FromQuery] string? status,
        [FromQuery] Guid? customerUserId,
        [FromQuery] Guid? serviceId,
        [FromQuery] DateTime? fromDateUtc,
        [FromQuery] DateTime? toDateUtc,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return ToActionResult(await jobsService.GetJobsAsync(new AdminJobListQuery(status, customerUserId, serviceId, fromDateUtc, toDateUtc, page, pageSize), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetJobDetails(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.GetJobDetailsAsync(id, cancellationToken));
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, AdminUpdateJobStatusRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.UpdateStatusAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNote(Guid id, AdminAddJobNoteRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.AddInternalNoteAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/assign-technician")]
    public async Task<IActionResult> AssignTechnician(Guid id, AssignTechnicianRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await jobsService.AssignTechnicianAsync(id, request, cancellationToken));
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
