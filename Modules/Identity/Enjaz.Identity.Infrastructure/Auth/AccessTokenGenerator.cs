using Enjaz.BuildingBlocks.Auth;
using Enjaz.Identity.Application.Auth;
using Microsoft.Extensions.Options;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class AccessTokenGenerator(
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtOptions) : IAccessTokenGenerator
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public int ExpiresInSeconds => _jwtOptions.AccessTokenMinutes * 60;

    public string GenerateAccessToken(Guid userId, string phoneNumber, IReadOnlyCollection<string> roles)
    {
        return jwtTokenGenerator.GenerateAccessToken(userId, phoneNumber, roles);
    }
}
