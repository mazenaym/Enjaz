using Enjaz.Customers.Domain.Customers;
using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Customers.Application.Customers;

public sealed class CustomerService(
    ICustomersRepository repository,
    ICurrentUserContext currentUserContext) : ICustomerService
{
    public async Task<Result<CustomerProfileResponse>> GetMyProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentProfileAsync(cancellationToken);
        return profile is null
            ? Result.Failure<CustomerProfileResponse>("customer_profile_not_found", "Customer profile was not found.")
            : Result.Success(Map(profile));
    }

    public async Task<Result<CustomerProfileResponse>> UpdateMyProfileAsync(UpdateCustomerProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentProfileAsync(cancellationToken);
        if (profile is null)
        {
            return Result.Failure<CustomerProfileResponse>("customer_profile_not_found", "Customer profile was not found.");
        }

        profile.FullName = request.FullName;
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(profile));
    }

    public async Task<Result<CustomerAddressResponse>> AddAddressAsync(CustomerAddressRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetProfileWithAddressesByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<CustomerAddressResponse>("customer_profile_not_found", "Customer profile was not found.");
        }

        var isFirstAddress = !profile.Addresses.Any();
        if (request.IsDefault || isFirstAddress)
        {
            foreach (var existingAddress in profile.Addresses)
            {
                existingAddress.IsDefault = false;
            }
        }

        var address = new CustomerAddress
        {
            CustomerId = profile.Id,
            Label = request.Label,
            City = request.City,
            Area = request.Area,
            Street = request.Street,
            BuildingNumber = request.BuildingNumber,
            Floor = request.Floor,
            Apartment = request.Apartment,
            Landmark = request.Landmark,
            FormattedAddress = request.FormattedAddress,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsDefault = request.IsDefault || isFirstAddress,
            CreatedAtUtc = DateTime.UtcNow
        };

        await repository.AddAddressAsync(address, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(address));
    }

    public async Task<Result<IReadOnlyCollection<CustomerAddressResponse>>> GetAddressesAsync(CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetProfileWithAddressesByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<IReadOnlyCollection<CustomerAddressResponse>>("customer_profile_not_found", "Customer profile was not found.");
        }

        return Result.Success<IReadOnlyCollection<CustomerAddressResponse>>(profile.Addresses.Select(Map).ToArray());
    }

    public async Task<Result<CustomerAddressResponse>> UpdateAddressAsync(Guid addressId, CustomerAddressRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetProfileWithAddressesByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<CustomerAddressResponse>("customer_profile_not_found", "Customer profile was not found.");
        }

        var address = profile.Addresses.FirstOrDefault(item => item.Id == addressId);
        if (address is null)
        {
            return Result.Failure<CustomerAddressResponse>("address_not_found", "Address was not found.");
        }

        if (request.IsDefault)
        {
            foreach (var existingAddress in profile.Addresses)
            {
                existingAddress.IsDefault = false;
            }
        }

        address.Label = request.Label;
        address.City = request.City;
        address.Area = request.Area;
        address.Street = request.Street;
        address.BuildingNumber = request.BuildingNumber;
        address.Floor = request.Floor;
        address.Apartment = request.Apartment;
        address.Landmark = request.Landmark;
        address.FormattedAddress = request.FormattedAddress;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.IsDefault = request.IsDefault || address.IsDefault;
        address.UpdatedAtUtc = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(address));
    }

    public async Task<Result> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetProfileWithAddressesByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("customer_profile_not_found", "Customer profile was not found.");
        }

        var address = profile.Addresses.FirstOrDefault(item => item.Id == addressId);
        if (address is null)
        {
            return Result.Failure("address_not_found", "Address was not found.");
        }

        var wasDefault = address.IsDefault;
        repository.RemoveAddress(address);

        if (wasDefault)
        {
            var nextDefault = profile.Addresses.FirstOrDefault(item => item.Id != address.Id);
            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetDefaultAddressAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetProfileWithAddressesByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("customer_profile_not_found", "Customer profile was not found.");
        }

        var address = profile.Addresses.FirstOrDefault(item => item.Id == addressId);
        if (address is null)
        {
            return Result.Failure("address_not_found", "Address was not found.");
        }

        foreach (var existingAddress in profile.Addresses)
        {
            existingAddress.IsDefault = existingAddress.Id == addressId;
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<CustomerProfile?> GetCurrentProfileAsync(CancellationToken cancellationToken)
    {
        return await repository.GetProfileByUserIdAsync(currentUserContext.UserId, cancellationToken);
    }

    private static CustomerProfileResponse Map(CustomerProfile profile)
    {
        return new CustomerProfileResponse(profile.Id, profile.UserId, profile.FullName, profile.PhoneNumber, profile.ProfileImageUrl);
    }

    private static CustomerAddressResponse Map(CustomerAddress address)
    {
        return new CustomerAddressResponse(
            address.Id,
            address.CustomerId,
            address.Label,
            address.City,
            address.Area,
            address.Street,
            address.BuildingNumber,
            address.Floor,
            address.Apartment,
            address.Landmark,
            address.FormattedAddress,
            address.Latitude,
            address.Longitude,
            address.IsDefault);
    }
}
