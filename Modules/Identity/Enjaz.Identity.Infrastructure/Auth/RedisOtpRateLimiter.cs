using Enjaz.Identity.Application.Auth;
using Microsoft.Extensions.Caching.Distributed;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class RedisOtpRateLimiter(IDistributedCache cache) : IOtpRateLimiter
{
    public async Task<bool> CanRequestOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var key = $"otp:req:{phoneNumber}";
        var value = await cache.GetStringAsync(key, cancellationToken);
        var count = string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);

        if (count >= 3)
        {
            return false;
        }

        await cache.SetStringAsync(
            key,
            (count + 1).ToString(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
            cancellationToken);

        return true;
    }
}
