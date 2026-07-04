using Enjaz.SharedKernel.Results;
using Enjaz.Support.Application.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Support.Endpoints.Support;

[ApiController]
[Authorize(Roles = "Admin")]
public sealed class AdminSupportController(ISupportService supportService) : ControllerBase
{
    [HttpGet("api/v1/admin/support/tickets")]
    public async Task<IActionResult> GetTickets([FromQuery] string? status, [FromQuery] string? priority, [FromQuery] Guid? relatedJobId, CancellationToken cancellationToken) => ToActionResult(await supportService.GetTicketsAsync(new SupportTicketQuery(status, priority, relatedJobId), cancellationToken));

    [HttpGet("api/v1/admin/support/tickets/{id:guid}")]
    public async Task<IActionResult> GetTicket(Guid id, CancellationToken cancellationToken) => ToActionResult(await supportService.GetTicketForAdminAsync(id, cancellationToken));

    [HttpPost("api/v1/admin/support/tickets/{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, AdminAssignTicketRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.AssignTicketAsync(id, request, cancellationToken));

    [HttpPost("api/v1/admin/support/tickets/{id:guid}/status")]
    public async Task<IActionResult> Status(Guid id, AdminUpdateTicketStatusRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.UpdateTicketStatusAsync(id, request, cancellationToken));

    [HttpPost("api/v1/admin/support/tickets/{id:guid}/messages")]
    public async Task<IActionResult> Message(Guid id, AddTicketMessageRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.AddAdminMessageAsync(id, request, cancellationToken));

    [HttpGet("api/v1/admin/support/disputes")]
    public async Task<IActionResult> GetDisputes([FromQuery] string? status, [FromQuery] Guid? jobId, CancellationToken cancellationToken) => ToActionResult(await supportService.GetDisputesAsync(new JobDisputeQuery(status, jobId), cancellationToken));

    [HttpPost("api/v1/admin/support/disputes/{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, ResolveDisputeRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.ResolveDisputeAsync(id, request, cancellationToken));

    [HttpPost("api/v1/admin/support/disputes/{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, ResolveDisputeRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.RejectDisputeAsync(id, request, cancellationToken));

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
