using Enjaz.Customers.Application.Customers;
using Enjaz.Customers.Domain.Customers;
using Enjaz.Customers.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Customers.Infrastructure.Customers;

public sealed class CustomersRepository(CustomersDbContext dbContext) : ICustomersRepository
{
    public async Task<CustomerProfile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CustomerProfiles.FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<CustomerProfile?> GetProfileWithAddressesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CustomerProfiles
            .Include(profile => profile.Addresses)
            .FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task<CustomerAddress?> GetAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CustomerAddresses.FirstOrDefaultAsync(
            address => address.CustomerId == customerId && address.Id == addressId,
            cancellationToken);
    }

    public async Task<int> CountAddressesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.CustomerAddresses.CountAsync(address => address.CustomerId == customerId, cancellationToken);
    }

    public async Task AddProfileAsync(CustomerProfile profile, CancellationToken cancellationToken = default)
    {
        await dbContext.CustomerProfiles.AddAsync(profile, cancellationToken);
    }

    public async Task AddAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default)
    {
        await dbContext.CustomerAddresses.AddAsync(address, cancellationToken);
    }

    public void RemoveAddress(CustomerAddress address)
    {
        dbContext.CustomerAddresses.Remove(address);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
