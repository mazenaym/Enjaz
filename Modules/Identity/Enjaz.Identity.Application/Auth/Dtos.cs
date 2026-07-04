namespace Enjaz.Identity.Application.Auth;

public sealed record RequestOtpRequest(string PhoneNumber, string Purpose);

public sealed record RequestOtpResponse(string Message, string? DevOtp);

public sealed record VerifyOtpRequest(string PhoneNumber, string Purpose, string Code);

public sealed record RegisterCustomerRequest(
    string FullName,
    string PhoneNumber,
    string? Email,
    string Password,
    string ConfirmPassword);

public sealed record RegisterCustomerResponse(string Message, bool RequiresPhoneVerification, string? DevOtp);

public sealed record VerifyPhoneRequest(string PhoneNumber, string OtpCode);

public sealed record ResendPhoneOtpRequest(string PhoneNumber);

public sealed record LoginRequest(string Identifier, string Password);

public sealed record LoginCustomerRequest(string Identifier, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record LogoutRequest(string RefreshToken);

public sealed record AuthUserResponse(Guid Id, string PhoneNumber, string? Email, string? FullName, IReadOnlyCollection<string> Roles);

public sealed record AuthResponse(string AccessToken, string RefreshToken, int ExpiresIn, AuthUserResponse User);
