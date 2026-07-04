using Enjaz.Identity.Application.Auth;
using Enjaz.Identity.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class UserPasswordHasher : IUserPasswordHasher
{
    private readonly PasswordHasher<User> passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
