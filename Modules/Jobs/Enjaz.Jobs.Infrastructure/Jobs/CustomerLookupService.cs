using Enjaz.Customers.Application.Customers;
using Enjaz.Customers.Infrastructure.Persistence;
using Enjaz.Jobs.Application.Jobs;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Jobs.Infrastructure.Jobs;

public sealed class CustomerLookupService(ICustomersRepository customersRepository, CustomersDbContext customersDbContext) : ICustomerLookupService
{
    public async Task<CustomerProfileLookupResult?> GetCustomerProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await customersRepository.GetProfileByUserIdAsync(userId, cancellationToken);
        return profile is null ? null : new CustomerProfileLookupResult(profile.Id, profile.UserId);
    }

    public async Task<bool> AddressBelongsToCustomerAsync(Guid customerUserId, Guid addressId, CancellationToken cancellationToken = default)
    {
        var profile = await customersRepository.GetProfileByUserIdAsync(customerUserId, cancellationToken);
        if (profile is null)
        {
            return false;
        }

        return await customersRepository.GetAddressAsync(profile.Id, addressId, cancellationToken) is not null;
    }

    public async Task<CustomerAddressLocationResult?> GetCustomerAddressLocationAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        return await customersDbContext.CustomerAddresses
            .AsNoTracking()
            .Where(address => address.Id == addressId)
            .Select(address => new CustomerAddressLocationResult(
                address.Latitude == 0 ? null : address.Latitude,
                address.Longitude == 0 ? null : address.Longitude))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
