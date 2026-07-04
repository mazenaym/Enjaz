using Enjaz.Identity.Domain.Users;
using Enjaz.Identity.Infrastructure.Persistence;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class IdentityUserService(IdentityDbContext dbContext) : IIdentityUserService
{
    public async Task<Result<IdentityUserInfo>> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .Include(item => item.UserRoles)
            .ThenInclude(item => item.Role)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<IdentityUserInfo>("user_not_found", "User was not found.");
        }

        return Result.Success(new IdentityUserInfo(
            user.Id,
            user.PhoneNumber,
            user.Email,
            user.FullName,
            user.IsPhoneVerified,
            user.UserRoles.Select(userRole => userRole.Role.Name).ToArray()));
    }

    public async Task<Result<bool>> IsPhoneVerifiedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);
        return user is null
            ? Result.Failure<bool>("user_not_found", "User was not found.")
            : Result.Success(user.IsPhoneVerified);
    }

    public async Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var normalizedRoleName = roleName.Trim().ToUpperInvariant();
        var user = await dbContext.Users
            .Include(item => item.UserRoles)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure("user_not_found", "User was not found.");
        }

        var role = await dbContext.Roles.FirstOrDefaultAsync(item => item.NormalizedName == normalizedRoleName, cancellationToken);
        if (role is null)
        {
            return Result.Failure("role_not_found", "Role was not found.");
        }

        if (user.UserRoles.Any(userRole => userRole.RoleId == role.Id))
        {
            return Result.Success();
        }

        dbContext.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
