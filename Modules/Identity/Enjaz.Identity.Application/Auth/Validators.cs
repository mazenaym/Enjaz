using FluentValidation;

namespace Enjaz.Identity.Application.Auth;

internal static class PhoneRules
{
    public const string EgyptianMobilePattern = @"^01[0125][0-9]{8}$";
}

public sealed class RequestOtpRequestValidator : AbstractValidator<RequestOtpRequest>
{
    public RequestOtpRequestValidator()
    {
        RuleFor(request => request.PhoneNumber).NotEmpty().Matches(PhoneRules.EgyptianMobilePattern);
        RuleFor(request => request.Purpose).NotEmpty().Must(BeValidPurpose);
    }

    private static bool BeValidPurpose(string purpose)
    {
        var normalized = purpose.Replace(" ", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
        return normalized is "VERIFYPHONE" or "RESETPASSWORD" or "CHANGEPHONE";
    }
}

public sealed class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(request => request.PhoneNumber).NotEmpty().Matches(PhoneRules.EgyptianMobilePattern);
        RuleFor(request => request.Purpose).NotEmpty().Must(BeValidPurpose);
        RuleFor(request => request.Code).NotEmpty().Length(6).Matches("^[0-9]{6}$");
    }

    private static bool BeValidPurpose(string purpose)
    {
        var normalized = purpose.Replace(" ", string.Empty, StringComparison.Ordinal).ToUpperInvariant();
        return normalized is "VERIFYPHONE" or "RESETPASSWORD" or "CHANGEPHONE";
    }
}

public sealed class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerRequestValidator()
    {
        RuleFor(request => request.FullName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.PhoneNumber).NotEmpty().Matches(PhoneRules.EgyptianMobilePattern);
        RuleFor(request => request.Email).EmailAddress().MaximumLength(320).When(request => !string.IsNullOrWhiteSpace(request.Email));
        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        RuleFor(request => request.ConfirmPassword).Equal(request => request.Password);
    }
}

public sealed class VerifyPhoneRequestValidator : AbstractValidator<VerifyPhoneRequest>
{
    public VerifyPhoneRequestValidator()
    {
        RuleFor(request => request.PhoneNumber).NotEmpty().Matches(PhoneRules.EgyptianMobilePattern);
        RuleFor(request => request.OtpCode).NotEmpty().Length(6).Matches("^[0-9]{6}$");
    }
}

public sealed class ResendPhoneOtpRequestValidator : AbstractValidator<ResendPhoneOtpRequest>
{
    public ResendPhoneOtpRequestValidator()
    {
        RuleFor(request => request.PhoneNumber).NotEmpty().Matches(PhoneRules.EgyptianMobilePattern);
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Identifier).NotEmpty().MaximumLength(320);
        RuleFor(request => request.Password).NotEmpty();
    }
}

public sealed class LoginCustomerRequestValidator : AbstractValidator<LoginCustomerRequest>
{
    public LoginCustomerRequestValidator()
    {
        RuleFor(request => request.Identifier).NotEmpty().MaximumLength(320);
        RuleFor(request => request.Password).NotEmpty();
    }
}

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(request => request.RefreshToken).NotEmpty();
    }
}

public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(request => request.RefreshToken).NotEmpty();
    }
}
