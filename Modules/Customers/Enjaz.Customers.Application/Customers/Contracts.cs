using Enjaz.Customers.Domain.Customers;
using Enjaz.SharedKernel.Results;

namespace Enjaz.Customers.Application.Customers;

public interface ICustomerService
{
    Task<Result<CustomerProfileResponse>> GetMyProfileAsync(CancellationToken cancellationToken = default);

    Task<Result<CustomerProfileResponse>> UpdateMyProfileAsync(UpdateCustomerProfileRequest request, CancellationToken cancellationToken = default);

    Task<Result<CustomerAddressResponse>> AddAddressAsync(CustomerAddressRequest request, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<CustomerAddressResponse>>> GetAddressesAsync(CancellationToken cancellationToken = default);

    Task<Result<CustomerAddressResponse>> UpdateAddressAsync(Guid addressId, CustomerAddressRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken = default);

    Task<Result> SetDefaultAddressAsync(Guid addressId, CancellationToken cancellationToken = default);
}

public interface ICustomersRepository
{
    Task<CustomerProfile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CustomerProfile?> GetProfileWithAddressesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CustomerAddress?> GetAddressAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default);

    Task<int> CountAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task AddProfileAsync(CustomerProfile profile, CancellationToken cancellationToken = default);

    Task AddAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default);

    void RemoveAddress(CustomerAddress address);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
