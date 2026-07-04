namespace Enjaz.SharedKernel.Customers;

public interface ICustomerProfileProvisioner
{
    Task EnsureCustomerProfileAsync(
        Guid userId,
        string phoneNumber,
        string fullName,
        CancellationToken cancellationToken = default);
}
