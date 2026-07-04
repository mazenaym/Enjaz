using Enjaz.Notifications.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Notifications.Application.Notifications;

public sealed class NotificationTemplateRenderer(INotificationsRepository repository) : INotificationTemplateRenderer
{
    public async Task<RenderedNotificationTemplate> RenderAsync(string type, string channel, IReadOnlyDictionary<string, string?> data, string language = "ar", string? fallbackTitle = null, string? fallbackBody = null, CancellationToken cancellationToken = default)
    {
        var template = await repository.QueryTemplates()
            .AsNoTracking()
            .Where(item => item.Type == type && item.Channel == channel && item.Language == language && item.IsActive)
            .OrderByDescending(item => item.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var title = template?.TitleTemplate ?? fallbackTitle ?? type;
        var body = template?.BodyTemplate ?? fallbackBody ?? type;
        return new RenderedNotificationTemplate(Render(title, data), Render(body, data));
    }

    private static string Render(string template, IReadOnlyDictionary<string, string?> data)
    {
        var rendered = template;
        foreach (var item in data)
        {
            rendered = rendered.Replace("{{" + item.Key + "}}", item.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return rendered;
    }
}
