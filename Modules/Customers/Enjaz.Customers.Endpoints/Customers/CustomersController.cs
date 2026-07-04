using Enjaz.Customers.Application.Customers;
using Enjaz.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enjaz.Customers.Endpoints.Customers;

[ApiController]
[Authorize]
[Route("api/v1/customers")]
public sealed class CustomersController(ICustomerService customerService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.GetMyProfileAsync(cancellationToken));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(UpdateCustomerProfileRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.UpdateMyProfileAsync(request, cancellationToken));
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.GetAddressesAsync(cancellationToken));
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress(CustomerAddressRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.AddAddressAsync(request, cancellationToken));
    }

    [HttpPut("addresses/{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, CustomerAddressRequest request, CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.UpdateAddressAsync(id, request, cancellationToken));
    }

    [HttpDelete("addresses/{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.DeleteAddressAsync(id, cancellationToken));
    }

    [HttpPost("addresses/{id:guid}/set-default")]
    public async Task<IActionResult> SetDefaultAddress(Guid id, CancellationToken cancellationToken)
    {
        return ToActionResult(await customerService.SetDefaultAddressAsync(id, cancellationToken));
    }

    private static IActionResult ToActionResult(Result result)
    {
        return result.IsSuccess
            ? new OkObjectResult(new { message = "Success" })
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }

    private static IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : new BadRequestObjectResult(new { code = result.ErrorCode, message = result.ErrorMessage });
    }
}
