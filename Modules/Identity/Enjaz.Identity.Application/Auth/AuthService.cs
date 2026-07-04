using Enjaz.Identity.Domain.Otp;
using Enjaz.Identity.Domain.Tokens;
using Enjaz.Identity.Domain.Users;
using Enjaz.SharedKernel.Customers;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Identity.Application.Auth;

public sealed class AuthService(
    IIdentityRepository repository,
    IOtpHasher otpHasher,
    IOtpRateLimiter otpRateLimiter,
    ISmsSender smsSender,
    IRefreshTokenService refreshTokenService,
    IAccessTokenGenerator accessTokenGenerator,
    IUserPasswordHasher passwordHasher,
    ICustomerProfileProvisioner customerProfileProvisioner,
    IAppEnvironment environment) : IAuthService
{
    private const string CustomerRole = "CUSTOMER";
    private const string VerifyPhonePurpose = "VERIFYPHONE";

    public async Task<Result<RequestOtpResponse>> RequestOtpAsync(RequestOtpRequest request, CancellationToken cancellationToken = default)
    {
        var purpose = NormalizePurpose(request.Purpose);
        if (purpose is "LOGIN")
        {
            return Result.Failure<RequestOtpResponse>("otp_login_disabled", "OTP is not supported for normal login.");
        }

        return await CreateAndSendOtpAsync(request.PhoneNumber, purpose, cancellationToken);
    }

    public async Task<Result> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        return await VerifyOtpInternalAsync(request.PhoneNumber, NormalizePurpose(request.Purpose), request.Code, true, cancellationToken);
    }

    public async Task<Result<RegisterCustomerResponse>> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.PhoneExistsAsync(request.PhoneNumber, cancellationToken))
        {
            return Result.Failure<RegisterCustomerResponse>("phone_already_registered", "Phone number is already registered.");
        }

        var normalizedEmail = NormalizeOptional(request.Email);
        if (normalizedEmail is not null && await repository.NormalizedEmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return Result.Failure<RegisterCustomerResponse>("email_already_registered", "Email is already registered.");
        }

        var role = await repository.GetRoleByNormalizedNameAsync(CustomerRole, cancellationToken);
        if (role is null)
        {
            return Result.Failure<RegisterCustomerResponse>("customer_role_missing", "Customer role is not configured.");
        }

        var user = new User
        {
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Username = request.PhoneNumber,
            NormalizedUsername = request.PhoneNumber,
            IsPhoneVerified = false,
            IsEmailVerified = false,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        await repository.AddUserAsync(user, cancellationToken);
        await repository.AddUserRoleAsync(new UserRole { UserId = user.Id, RoleId = role.Id }, cancellationToken);

        var otpResult = await CreateOtpAsync(user.PhoneNumber, VerifyPhonePurpose, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure<RegisterCustomerResponse>(otpResult.ErrorCode!, otpResult.ErrorMessage!);
        }

        await repository.SaveChangesAsync(cancellationToken);
        await customerProfileProvisioner.EnsureCustomerProfileAsync(user.Id, user.PhoneNumber, request.FullName, cancellationToken);
        await smsSender.SendOtpAsync(user.PhoneNumber, otpResult.Value!, cancellationToken);

        return Result.Success(new RegisterCustomerResponse(
            "Customer registered successfully. Phone verification is required.",
            true,
            environment.IsDevelopment ? otpResult.Value : null));
    }

    public async Task<Result<AuthResponse>> VerifyPhoneAsync(VerifyPhoneRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByPhoneAsync(request.PhoneNumber, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthResponse>("invalid_phone_verification", "Phone verification request is invalid.");
        }

        var otpResult = await VerifyOtpInternalAsync(request.PhoneNumber, VerifyPhonePurpose, request.OtpCode, true, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure<AuthResponse>(otpResult.ErrorCode!, otpResult.ErrorMessage!);
        }

        user.IsPhoneVerified = true;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var roles = user.UserRoles.Select(userRole => userRole.Role.Name).ToArray();
        var authResponse = await CreateAuthResponseAsync(user, roles, ipAddress, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(authResponse);
    }

    public async Task<Result<RequestOtpResponse>> ResendPhoneOtpAsync(ResendPhoneOtpRequest request, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetUserByPhoneAsync(request.PhoneNumber, cancellationToken);
        if (user is null)
        {
            return Result.Success(new RequestOtpResponse("If the phone number exists, an OTP will be sent.", null));
        }

        if (user.IsPhoneVerified)
        {
            return Result.Success(new RequestOtpResponse("Phone number is already verified.", null));
        }

        return await CreateAndSendOtpAsync(request.PhoneNumber, VerifyPhonePurpose, cancellationToken);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await FindUserByIdentifierAsync(request.Identifier, cancellationToken);
        if (user is null || !user.IsActive || !passwordHasher.VerifyPassword(user, request.Password))
        {
            return Result.Failure<AuthResponse>("invalid_login", "Invalid identifier or password.");
        }

        if (!user.IsPhoneVerified)
        {
            return Result.Failure<AuthResponse>("phone_not_verified", "Phone number must be verified before login.");
        }

        var roles = user.UserRoles.Select(userRole => userRole.Role.Name).ToArray();
        if (!roles.Any(role => string.Equals(role, "Customer", StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Failure<AuthResponse>("invalid_login", "Invalid identifier or password.");
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        user.UpdatedAtUtc = DateTime.UtcNow;

        var authResponse = await CreateAuthResponseAsync(user, roles, ipAddress, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> LoginCustomerAsync(LoginCustomerRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        return await LoginAsync(new LoginRequest(request.Identifier, request.Password), ipAddress, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var tokenHash = refreshTokenService.Hash(request.RefreshToken);
        var refreshToken = await repository.GetRefreshTokenByHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null || refreshToken.RevokedAtUtc is not null || refreshToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result.Failure<AuthResponse>("invalid_refresh_token", "Refresh token is invalid.");
        }

        var user = await repository.GetUserWithRolesByIdAsync(refreshToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthResponse>("invalid_refresh_token", "Refresh token is invalid.");
        }

        var roles = user.UserRoles.Select(userRole => userRole.Role.Name).ToArray();
        var newPlainRefreshToken = refreshTokenService.GenerateToken();
        var newRefreshTokenHash = refreshTokenService.Hash(newPlainRefreshToken);

        refreshToken.RevokedAtUtc = DateTime.UtcNow;
        refreshToken.ReplacedByTokenHash = newRefreshTokenHash;

        await repository.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAtUtc = refreshTokenService.CreateExpirationUtc(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIp = ipAddress
        }, cancellationToken);

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(BuildAuthResponse(user, roles, newPlainRefreshToken));
    }

    public async Task<Result> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = refreshTokenService.Hash(request.RefreshToken);
        var refreshToken = await repository.GetRefreshTokenByHashAsync(tokenHash, cancellationToken);
        if (refreshToken is null)
        {
            return Result.Success();
        }

        refreshToken.RevokedAtUtc ??= DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result<RequestOtpResponse>> CreateAndSendOtpAsync(string phoneNumber, string purpose, CancellationToken cancellationToken)
    {
        if (!await otpRateLimiter.CanRequestOtpAsync(phoneNumber, cancellationToken))
        {
            return Result.Failure<RequestOtpResponse>("otp_rate_limited", "Too many OTP requests. Please try again later.");
        }

        var otpResult = await CreateOtpAsync(phoneNumber, purpose, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure<RequestOtpResponse>(otpResult.ErrorCode!, otpResult.ErrorMessage!);
        }

        await repository.SaveChangesAsync(cancellationToken);
        await smsSender.SendOtpAsync(phoneNumber, otpResult.Value!, cancellationToken);

        return Result.Success(new RequestOtpResponse("OTP sent successfully", environment.IsDevelopment ? otpResult.Value : null));
    }

    private async Task<Result<string>> CreateOtpAsync(string phoneNumber, string purpose, CancellationToken cancellationToken)
    {
        var code = Random.Shared.Next(0, 1_000_000).ToString("D6");
        var otp = new OtpCode
        {
            PhoneNumber = phoneNumber,
            Purpose = purpose,
            CodeHash = otpHasher.Hash(phoneNumber, purpose, code),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5),
            MaxAttempts = 5,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddOtpAsync(otp, cancellationToken);
        return Result.Success(code);
    }

    private async Task<Result> VerifyOtpInternalAsync(string phoneNumber, string purpose, string code, bool markAsUsed, CancellationToken cancellationToken)
    {
        var otp = await repository.GetLatestUnusedOtpAsync(phoneNumber, purpose, cancellationToken);
        if (otp is null)
        {
            return Result.Failure("otp_not_found", "OTP code was not found.");
        }

        if (otp.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return Result.Failure("otp_expired", "OTP code has expired.");
        }

        if (otp.AttemptCount >= otp.MaxAttempts)
        {
            return Result.Failure("otp_max_attempts", "OTP maximum attempts exceeded.");
        }

        if (!otpHasher.Verify(phoneNumber, purpose, code, otp.CodeHash))
        {
            otp.AttemptCount++;
            await repository.SaveChangesAsync(cancellationToken);
            return Result.Failure("otp_invalid", "OTP code is invalid.");
        }

        if (markAsUsed)
        {
            otp.UsedAtUtc = DateTime.UtcNow;
            await repository.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private async Task<AuthResponse> CreateAuthResponseAsync(User user, IReadOnlyCollection<string> roles, string? ipAddress, CancellationToken cancellationToken)
    {
        var plainRefreshToken = refreshTokenService.GenerateToken();
        await repository.AddRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenService.Hash(plainRefreshToken),
            ExpiresAtUtc = refreshTokenService.CreateExpirationUtc(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByIp = ipAddress
        }, cancellationToken);

        return BuildAuthResponse(user, roles, plainRefreshToken);
    }

    private AuthResponse BuildAuthResponse(User user, IReadOnlyCollection<string> roles, string refreshToken)
    {
        return new AuthResponse(
            accessTokenGenerator.GenerateAccessToken(user.Id, user.PhoneNumber, roles),
            refreshToken,
            accessTokenGenerator.ExpiresInSeconds,
            new AuthUserResponse(user.Id, user.PhoneNumber, user.Email, user.FullName, roles));
    }

    private async Task<User?> FindUserByIdentifierAsync(string identifier, CancellationToken cancellationToken)
    {
        var trimmed = identifier.Trim();
        if (trimmed.Contains('@', StringComparison.Ordinal))
        {
            var normalizedEmail = NormalizeOptional(trimmed);
            return normalizedEmail is null
                ? null
                : await repository.GetUserByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        }

        return await repository.GetUserByPhoneAsync(trimmed, cancellationToken);
    }

    private static string NormalizePurpose(string purpose)
    {
        return purpose.Replace(" ", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
