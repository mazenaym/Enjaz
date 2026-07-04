namespace Enjaz.BuildingBlocks.Auth;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(Guid userId, string phoneNumber, IReadOnlyCollection<string> roles);
}
