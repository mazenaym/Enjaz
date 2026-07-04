using System.Security.Cryptography;
using System.Text;
using Enjaz.BuildingBlocks.Auth;
using Enjaz.Identity.Application.Auth;
using Microsoft.Extensions.Options;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class RefreshTokenService(IOptions<JwtOptions> jwtOptions) : IRefreshTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public DateTime CreateExpirationUtc()
    {
        return DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);
    }
}
