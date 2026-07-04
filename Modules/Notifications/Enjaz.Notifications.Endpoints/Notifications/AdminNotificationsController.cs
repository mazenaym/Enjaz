using Enjaz.Notifications.Application.Notifications;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Notifications.Endpoints.Notifications;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/notifications")]
public sealed class AdminNotificationsController(IAdminNotificationsService notificationsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] Guid? userId, [FromQuery] string? type, [FromQuery] string? channel, [FromQuery] bool? isRead, [FromQuery] DateTime? fromDateUtc, [FromQuery] DateTime? toDateUtc, CancellationToken cancellationToken)
        => ToActionResult(await notificationsService.GetNotificationsAsync(new NotificationQuery(userId, type, channel, isRead, fromDateUtc, toDateUtc), cancellationToken));

    [HttpGet("delivery-logs")]
    public async Task<IActionResult> GetDeliveryLogs([FromQuery] string? status, [FromQuery] string? channel, [FromQuery] Guid? userId, CancellationToken cancellationToken)
        => ToActionResult(await notificationsService.GetDeliveryLogsAsync(new DeliveryLogQuery(status, channel, userId), cancellationToken));

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates(CancellationToken cancellationToken) => ToActionResult(await notificationsService.GetTemplatesAsync(cancellationToken));

    [HttpPost("templates")]
    public async Task<IActionResult> CreateTemplate(NotificationTemplateRequest request, CancellationToken cancellationToken) => ToActionResult(await notificationsService.CreateTemplateAsync(request, cancellationToken));

    [HttpPut("templates/{id:guid}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, NotificationTemplateRequest request, CancellationToken cancellationToken) => ToActionResult(await notificationsService.UpdateTemplateAsync(id, request, cancellationToken));

    [HttpPost("templates/{id:guid}/activate")]
    public async Task<IActionResult> ActivateTemplate(Guid id, CancellationToken cancellationToken) => ToActionResult(await notificationsService.SetTemplateActiveAsync(id, true, cancellationToken));

    [HttpPost("templates/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateTemplate(Guid id, CancellationToken cancellationToken) => ToActionResult(await notificationsService.SetTemplateActiveAsync(id, false, cancellationToken));

    [HttpPost("test")]
    public async Task<IActionResult> SendTest(AdminTestNotificationRequest request, CancellationToken cancellationToken) => ToActionResult(await notificationsService.SendTestAsync(request, cancellationToken));

    private static IActionResult ToActionResult(Result result) => result.IsSuccess ? new OkObjectResult(new { message = "Success" }) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    private static IActionResult ToActionResult<T>(Result<T> result) => result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
}
