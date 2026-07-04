namespace Enjaz.BuildingBlocks.Auth;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateAccessToken()
    {
        throw new NotImplementedException("JWT generation will be implemented with Identity use cases.");
    }
}
