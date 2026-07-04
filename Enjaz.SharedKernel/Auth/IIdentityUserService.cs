using Enjaz.SharedKernel.Results;

namespace Enjaz.SharedKernel.Auth;

public sealed record IdentityUserInfo(Guid Id, string PhoneNumber, string? Email, string? FullName, bool IsPhoneVerified, IReadOnlyCollection<string> Roles);

public interface IIdentityUserService
{
    Task<Result<IdentityUserInfo>> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsPhoneVerifiedAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
}
