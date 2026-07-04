using System.Text.Json;
using Enjaz.Notifications.Application.Notifications;
using Enjaz.Notifications.Domain.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Enjaz.Notifications.Infrastructure.Notifications;

public sealed class FakeSmsNotificationProvider(IConfiguration configuration, ILogger<FakeSmsNotificationProvider> logger) : ISmsNotificationProvider
{
    public string ProviderName => NotificationProviders.Fake;

    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fake SMS notification to {Recipient}: {Title}", request.Recipient, request.Title);
        return Task.FromResult(Result(request, FakeProviderConfiguration.IsEnabled(configuration["Notifications:Fake:FailSms"])));
    }

    private static ProviderNotificationResult Result(ProviderNotificationRequest request, bool fail) => fail
        ? new(false, NotificationProviders.Fake, null, "Fake SMS failure requested by configuration.")
        : new(true, NotificationProviders.Fake, JsonSerializer.Serialize(new { fake = true, request.Recipient }), null);
}

public sealed class FakePushNotificationProvider(IConfiguration configuration, ILogger<FakePushNotificationProvider> logger) : IPushNotificationProvider
{
    public string ProviderName => NotificationProviders.Fake;

    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fake push notification to {Recipient}: {Title}", request.Recipient, request.Title);
        return Task.FromResult(FakeProviderConfiguration.IsEnabled(configuration["Notifications:Fake:FailPush"])
            ? new ProviderNotificationResult(false, NotificationProviders.Fake, null, "Fake push failure requested by configuration.")
            : new ProviderNotificationResult(true, NotificationProviders.Fake, JsonSerializer.Serialize(new { fake = true, request.Recipient }), null));
    }
}

public sealed class FakeEmailNotificationProvider(IConfiguration configuration, ILogger<FakeEmailNotificationProvider> logger) : IEmailNotificationProvider
{
    public string ProviderName => NotificationProviders.Fake;

    public Task<ProviderNotificationResult> SendAsync(ProviderNotificationRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fake email notification to {Recipient}: {Title}", request.Recipient, request.Title);
        return Task.FromResult(FakeProviderConfiguration.IsEnabled(configuration["Notifications:Fake:FailEmail"])
            ? new ProviderNotificationResult(false, NotificationProviders.Fake, null, "Fake email failure requested by configuration.")
            : new ProviderNotificationResult(true, NotificationProviders.Fake, JsonSerializer.Serialize(new { fake = true, request.Recipient }), null));
    }
}

internal static class FakeProviderConfiguration
{
    public static bool IsEnabled(string? value) => bool.TryParse(value, out var parsed) && parsed;
}
