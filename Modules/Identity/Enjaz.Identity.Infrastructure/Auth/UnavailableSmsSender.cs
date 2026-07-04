using Enjaz.Identity.Application.Auth;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class UnavailableSmsSender : ISmsSender
{
    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("SMS provider is not configured for this environment.");
    }
}
