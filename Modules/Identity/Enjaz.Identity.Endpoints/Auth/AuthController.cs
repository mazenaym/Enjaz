using Enjaz.Identity.Application.Auth;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Identity.Endpoints.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("request-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestOtp(RequestOtpRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.RequestOtpAsync(request, cancellationToken));
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.VerifyOtpAsync(request, cancellationToken));
    }

    [HttpPost("register-customer")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterCustomer(RegisterCustomerRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.RegisterCustomerAsync(request, cancellationToken));
    }

    [HttpPost("verify-phone")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyPhone(VerifyPhoneRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.VerifyPhoneAsync(request, GetIpAddress(), cancellationToken));
    }

    [HttpPost("resend-phone-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendPhoneOtp(ResendPhoneOtpRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.ResendPhoneOtpAsync(request, cancellationToken));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.LoginAsync(request, GetIpAddress(), cancellationToken));
    }

    [HttpPost("login-customer")]
    [AllowAnonymous]
    [Obsolete("Use POST /api/v1/auth/login.")]
    public async Task<IActionResult> LoginCustomer(LoginCustomerRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.LoginCustomerAsync(request, GetIpAddress(), cancellationToken));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.RefreshTokenAsync(request, GetIpAddress(), cancellationToken));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await authService.LogoutAsync(request, cancellationToken));
    }

    private string? GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static IActionResult ToActionResult(Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(new { message = "Success" })
            : ToErrorActionResult(result.ErrorCode, result.ErrorMessage);
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : ToErrorActionResult(result.ErrorCode, result.ErrorMessage);
    }

    private static IActionResult ToErrorActionResult(string? code, string? message)
    {
        var body = new { code, message };
        return code switch
        {
            "invalid_login" or "invalid_refresh_token" => new UnauthorizedObjectResult(body),
            "phone_not_verified" => new ObjectResult(body) { StatusCode = StatusCodes.Status403Forbidden },
            not null when code.EndsWith("_already_registered", StringComparison.Ordinal) => new ConflictObjectResult(body),
            not null when code.EndsWith("_not_found", StringComparison.Ordinal) => new NotFoundObjectResult(body),
            _ => new BadRequestObjectResult(body)
        };
    }
}
