using Enjaz.Notifications.Domain.Notifications;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Notifications.Application.Notifications;

public interface INotificationService
{
    Task<Result> SendInAppAsync(Guid userId, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default);
    Task<Result> SendSmsAsync(Guid userId, string phoneNumber, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default);
    Task<Result> SendPushAsync(Guid userId, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default);
    Task<Result> SendEmailAsync(Guid userId, string email, string type, string title, string body, IReadOnlyDictionary<string, string?>? data = null, CancellationToken cancellationToken = default);
    Task<Result> SendToUserAsync(Guid userId, string type, IReadOnlyDictionary<string, string?> templateData, IReadOnlyCollection<string> channels, CancellationToken cancellationToken = default);
    Task<Result> SendAsync(SendNotificationRequest request, CancellationToken cancellationToken = default);
}

public interface IUserNotificationsService
{
    Task<Result<IReadOnlyCollection<NotificationResponse>>> GetMyNotificationsAsync(CancellationToken cancellationToken = default);
    Task<Result> MarkReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> MarkAllReadAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<NotificationPreferenceResponse>>> GetMyPreferencesAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<NotificationPreferenceResponse>>> UpdateMyPreferencesAsync(UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken = default);
    Task<Result> RegisterDeviceTokenAsync(RegisterDeviceTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteDeviceTokenAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IAdminNotificationsService
{
    Task<Result<IReadOnlyCollection<NotificationResponse>>> GetNotificationsAsync(NotificationQuery query, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<NotificationDeliveryLogResponse>>> GetDeliveryLogsAsync(DeliveryLogQuery query, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<NotificationTemplateResponse>>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<NotificationTemplateResponse>> CreateTemplateAsync(NotificationTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<NotificationTemplateResponse>> UpdateTemplateAsync(Guid id, NotificationTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetTemplateActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default);
    Task<Result> SendTestAsync(AdminTestNotificationRequest request, CancellationToken cancellationToken = default);
}

public interface IRetryFailedNotificationsJob
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface INotificationTemplateRenderer
{
    Task<RenderedNotificationTemplate> RenderAsync(string type, string channel, IReadOnlyDictionary<string, string?> data, string language = "ar", string? fallbackTitle = null, string? fallbackBody = null, CancellationToken cancellationToken = default);
}

public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationDeliveryLog log, CancellationToken cancellationToken = default);
}

public interface ISmsNotificationProvider
{
    string ProviderName { get; }
    Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default);
}

public interface IPushNotificationProvider
{
    string ProviderName { get; }
    Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default);
}

public interface IEmailNotificationProvider
{
    string ProviderName { get; }
    Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default);
}

public interface INotificationsRepository
{
    IQueryable<Notification> QueryNotifications();
    IQueryable<NotificationTemplate> QueryTemplates();
    IQueryable<NotificationDeliveryLog> QueryDeliveryLogs();
    IQueryable<UserNotificationPreference> QueryPreferences();
    IQueryable<PushDeviceToken> QueryDeviceTokens();
    Task<Notification?> GetNotificationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetTemplateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<NotificationDeliveryLog?> GetDeliveryLogAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserNotificationPreference?> GetPreferenceAsync(Guid userId, string channel, CancellationToken cancellationToken = default);
    Task<PushDeviceToken?> GetDeviceTokenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PushDeviceToken?> GetDeviceTokenByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    Task AddTemplateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task AddDeliveryLogAsync(NotificationDeliveryLog log, CancellationToken cancellationToken = default);
    Task AddPreferenceAsync(UserNotificationPreference preference, CancellationToken cancellationToken = default);
    Task AddDeviceTokenAsync(PushDeviceToken token, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
