using Enjaz.SharedKernel.Results;
using Enjaz.Technicians.Application.Technicians;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Technicians.Endpoints.Technicians;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/technicians")]
public sealed class AdminTechniciansController(ITechnicianService technicianService) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.GetPendingAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.GetDetailsAsync(id, cancellationToken));
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.ApproveAsync(id, cancellationToken));
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, RejectTechnicianRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.RejectAsync(id, request.Reason, cancellationToken));
    }

    [HttpPost("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, RejectTechnicianRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.SuspendAsync(id, request.Reason, cancellationToken));
    }

    [HttpPost("documents/{documentId:guid}/approve")]
    public async Task<IActionResult> ApproveDocument(Guid documentId, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.ApproveDocumentAsync(documentId, cancellationToken));
    }

    [HttpPost("documents/{documentId:guid}/reject")]
    public async Task<IActionResult> RejectDocument(Guid documentId, RejectTechnicianRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await technicianService.RejectDocumentAsync(documentId, request.Reason, cancellationToken));
    }

    private static IActionResult ToActionResult(Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(new { message = "Success" })
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
