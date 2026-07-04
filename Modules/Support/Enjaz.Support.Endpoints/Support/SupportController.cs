using Enjaz.SharedKernel.Results;
using Enjaz.Support.Application.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Support.Endpoints.Support;

[ApiController]
public sealed class SupportController(ISupportService supportService) : ControllerBase
{
    [Authorize(Roles = "Customer,Technician")]
    [HttpPost("api/v1/support/tickets")]
    public async Task<IActionResult> CreateTicket(CreateSupportTicketRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.CreateTicketAsync(request, cancellationToken));

    [Authorize(Roles = "Customer,Technician")]
    [HttpGet("api/v1/support/tickets/my")]
    public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken) => ToActionResult(await supportService.GetMyTicketsAsync(cancellationToken));

    [Authorize(Roles = "Customer,Technician")]
    [HttpGet("api/v1/support/tickets/{id:guid}")]
    public async Task<IActionResult> GetTicket(Guid id, CancellationToken cancellationToken) => ToActionResult(await supportService.GetTicketAsync(id, cancellationToken));

    [Authorize(Roles = "Customer,Technician")]
    [HttpPost("api/v1/support/tickets/{id:guid}/messages")]
    public async Task<IActionResult> AddMessage(Guid id, AddTicketMessageRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.AddMessageAsync(id, request, cancellationToken));

    [Authorize(Roles = "Customer,Technician")]
    [HttpPost("api/v1/jobs/{jobId:guid}/disputes")]
    public async Task<IActionResult> OpenDispute(Guid jobId, OpenDisputeRequest request, CancellationToken cancellationToken) => ToActionResult(await supportService.OpenDisputeAsync(jobId, request, cancellationToken));

    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
