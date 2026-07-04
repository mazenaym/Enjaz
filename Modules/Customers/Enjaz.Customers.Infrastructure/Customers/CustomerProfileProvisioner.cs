using Enjaz.Customers.Domain.Customers;
using Enjaz.Customers.Infrastructure.Persistence;
using Enjaz.SharedKernel.Customers;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Customers.Infrastructure.Customers;

public sealed class CustomerProfileProvisioner(CustomersDbContext dbContext) : ICustomerProfileProvisioner
{
    public async Task EnsureCustomerProfileAsync(
        Guid userId,
        string phoneNumber,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.CustomerProfiles.AnyAsync(profile => profile.UserId == userId, cancellationToken);
        if (exists)
        {
            return;
        }

        await dbContext.CustomerProfiles.AddAsync(new CustomerProfile
        {
            UserId = userId,
            PhoneNumber = phoneNumber,
            FullName = fullName,
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
