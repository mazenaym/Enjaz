using Enjaz.Notifications.Application.Notifications;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Notifications.Endpoints.Notifications;

[ApiController]
[Authorize(Roles = "Technician")]
[Route("api/v1/technicians")]
public sealed class TechnicianNotificationsController(IUserNotificationsService notificationsService) : ControllerBase
{
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications(CancellationToken cancellationToken) => ToActionResult(await notificationsService.GetMyNotificationsAsync(cancellationToken));

    [HttpPost("notifications/{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken) => ToActionResult(await notificationsService.MarkReadAsync(id, cancellationToken));

    [HttpPost("device-tokens")]
    public async Task<IActionResult> RegisterDeviceToken(RegisterDeviceTokenRequest request, CancellationToken cancellationToken) => ToActionResult(await notificationsService.RegisterDeviceTokenAsync(request, cancellationToken));

    private static IActionResult ToActionResult(Result result) => result.IsSuccess ? new OkObjectResult(new { message = "Success" }) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
