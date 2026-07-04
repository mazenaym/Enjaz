using Enjaz.Identity.Domain.Otp;
using Enjaz.Identity.Domain.Tokens;
using Enjaz.Identity.Domain.Users;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Identity.Application.Auth;

public interface IAuthService
{
    Task<Result<RequestOtpResponse>> RequestOtpAsync(RequestOtpRequest request, CancellationToken cancellationToken = default);

    Task<Result> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);

    Task<Result<RegisterCustomerResponse>> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> VerifyPhoneAsync(VerifyPhoneRequest request, string? ipAddress, CancellationToken cancellationToken = default);

    Task<Result<RequestOtpResponse>> ResendPhoneOtpAsync(ResendPhoneOtpRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> LoginCustomerAsync(LoginCustomerRequest request, string? ipAddress, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}

public interface IIdentityRepository
{
    Task<User?> GetUserByPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default);

    Task<User?> GetUserByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<User?> GetUserWithRolesByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default);

    Task<bool> NormalizedEmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    Task<OtpCode?> GetLatestUnusedOtpAsync(string phoneNumber, string purpose, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddOtpAsync(OtpCode otpCode, CancellationToken cancellationToken = default);

    Task AddUserAsync(User user, CancellationToken cancellationToken = default);

    Task AddUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IOtpHasher
{
    string Hash(string phoneNumber, string purpose, string code);

    bool Verify(string phoneNumber, string purpose, string code, string hash);
}

public interface IRefreshTokenService
{
    string GenerateToken();

    string Hash(string token);

    DateTime CreateExpirationUtc();
}

public interface IUserPasswordHasher
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string password);
}

public interface IAccessTokenGenerator
{
    int ExpiresInSeconds { get; }

    string GenerateAccessToken(Guid userId, string phoneNumber, IReadOnlyCollection<string> roles);
}

public interface IOtpRateLimiter
{
    Task<bool> CanRequestOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);
}

public interface ISmsSender
{
    Task SendOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken = default);
}

public interface IAppEnvironment
{
    bool IsDevelopment { get; }
}
