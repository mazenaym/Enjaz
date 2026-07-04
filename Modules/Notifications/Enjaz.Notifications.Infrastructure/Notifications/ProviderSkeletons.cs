using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Microsoft.Extensions.Configuration;

namespace Enjaz.Notifications.Infrastructure.Notifications;

public sealed class VictoryLinkSmsProvider(IConfiguration configuration) : ISmsNotificationProvider
{
    public string ProviderName => NotificationProviders.VictoryLink;
    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(MissingIfNeeded(request, "Notifications:VictoryLink:BaseUrl", "Victory Link SMS provider is not configured."));
    private ProviderNotificationResult MissingIfNeeded(ProviderNotificationRequest request, string key, string message) => string.IsNullOrWhiteSpace(configuration[key]) ? new(false, ProviderName, null, message) : new(false, ProviderName, null, "Victory Link SMS provider skeleton does not send real SMS yet.");
}

public sealed class CequensSmsProvider(IConfiguration configuration) : ISmsNotificationProvider
{
    public string ProviderName => NotificationProviders.Cequens;
    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(string.IsNullOrWhiteSpace(configuration["Notifications:Cequens:ApiKey"]) ? new ProviderNotificationResult(false, ProviderName, null, "CEQUENS SMS provider is not configured.") : new ProviderNotificationResult(false, ProviderName, null, "CEQUENS SMS provider skeleton does not send real SMS yet."));
}

public sealed class ExpoPushProvider(IConfiguration configuration) : IPushNotificationProvider
{
    public string ProviderName => NotificationProviders.Expo;
    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(string.IsNullOrWhiteSpace(configuration["Notifications:Expo:BaseUrl"]) ? new ProviderNotificationResult(false, ProviderName, null, "Expo push provider is not configured.") : new ProviderNotificationResult(false, ProviderName, null, "Expo push provider skeleton does not send real push yet."));
}

public sealed class FirebasePushProvider(IConfiguration configuration) : IPushNotificationProvider
{
    public string ProviderName => NotificationProviders.Firebase;
    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(string.IsNullOrWhiteSpace(configuration["Notifications:Firebase:ProjectId"]) ? new ProviderNotificationResult(false, ProviderName, null, "Firebase push provider is not configured.") : new ProviderNotificationResult(false, ProviderName, null, "Firebase push provider skeleton does not send real push yet."));
}

public sealed class SmtpEmailProvider(IConfiguration configuration) : IEmailNotificationProvider
{
    public string ProviderName => NotificationProviders.Email;
    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult(string.IsNullOrWhiteSpace(configuration["Notifications:Smtp:Host"]) ? new ProviderNotificationResult(false, ProviderName, null, "SMTP email provider is not configured.") : new ProviderNotificationResult(false, ProviderName, null, "SMTP email provider skeleton does not send real email yet."));
}
