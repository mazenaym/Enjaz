using Enjaz.Identity.Application.Auth;
using Enjaz.Identity.Domain.Otp;
using Enjaz.Identity.Domain.Tokens;
using Enjaz.Identity.Domain.Users;
using Enjaz.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class IdentityRepository(IdentityDbContext dbContext) : IIdentityRepository
{
    public async Task<User?> GetUserByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<User?> GetUserByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<User?> GetUserWithRolesByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AnyAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> NormalizedEmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Roles.FirstOrDefaultAsync(role => role.NormalizedName == normalizedName, cancellationToken);
    }

    public async Task<OtpCode?> GetLatestUnusedOtpAsync(string phoneNumber, string purpose, CancellationToken cancellationToken = default)
    {
        return await dbContext.OtpCodes
            .Where(otp => otp.PhoneNumber == phoneNumber && otp.Purpose == purpose && otp.UsedAtUtc == null)
            .OrderByDescending(otp => otp.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task AddOtpAsync(OtpCode otpCode, CancellationToken cancellationToken = default)
    {
        await dbContext.OtpCodes.AddAsync(otpCode, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        await dbContext.UserRoles.AddAsync(userRole, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
