namespace Enjaz.BuildingBlocks.Auth;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken();
}
