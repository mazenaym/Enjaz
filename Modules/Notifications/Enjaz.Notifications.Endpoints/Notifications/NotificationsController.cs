using Enjaz.Notifications.Application.Notifications;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Notifications.Endpoints.Notifications;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/v1/notifications")]
public sealed class NotificationsController(IUserNotificationsService notificationsService) : ControllerBase
{
    [HttpGet("my")]
    public async Task<IActionResult> GetMy(CancellationToken cancellationToken) => ToActionResult(await notificationsService.GetMyNotificationsAsync(cancellationToken));

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken) => ToActionResult(await notificationsService.MarkReadAsync(id, cancellationToken));

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken) => ToActionResult(await notificationsService.MarkAllReadAsync(cancellationToken));

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences(CancellationToken cancellationToken) => ToActionResult(await notificationsService.GetMyPreferencesAsync(cancellationToken));

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken) => ToActionResult(await notificationsService.UpdateMyPreferencesAsync(request, cancellationToken));

    [HttpPost("device-tokens")]
    public async Task<IActionResult> RegisterDeviceToken(RegisterDeviceTokenRequest request, CancellationToken cancellationToken) => ToActionResult(await notificationsService.RegisterDeviceTokenAsync(request, cancellationToken));

    [HttpDelete("device-tokens/{id:guid}")]
    public async Task<IActionResult> DeleteDeviceToken(Guid id, CancellationToken cancellationToken) => ToActionResult(await notificationsService.DeleteDeviceTokenAsync(id, cancellationToken));

    private static IActionResult ToActionResult(Result result) => result.IsSuccess ? new OkObjectResult(new { message = "Success" }) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
