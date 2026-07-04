using Enjaz.Identity.Application.Auth;
using Microsoft.Extensions.Logging;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class FakeSmsSender(ILogger<FakeSmsSender> logger) : ISmsSender
{
    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Development OTP for {PhoneNumber}: {OtpCode}", phoneNumber, code);
        return Task.CompletedTask;
    }
}
