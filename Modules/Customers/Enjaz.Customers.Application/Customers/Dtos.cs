namespace Enjaz.Customers.Application.Customers;

public sealed record CustomerProfileResponse(Guid Id, Guid UserId, string FullName, string PhoneNumber, string? ProfileImageUrl);

public sealed record UpdateCustomerProfileRequest(string FullName);

public sealed record CustomerAddressRequest(
    string? Label,
    string City,
    string Area,
    string Street,
    string? BuildingNumber,
    string? Floor,
    string? Apartment,
    string? Landmark,
    string? FormattedAddress,
    decimal Latitude,
    decimal Longitude,
    bool IsDefault);

public sealed record CustomerAddressResponse(
    Guid Id,
    Guid CustomerId,
    string? Label,
    string City,
    string Area,
    string Street,
    string? BuildingNumber,
    string? Floor,
    string? Apartment,
    string? Landmark,
    string? FormattedAddress,
    decimal Latitude,
    decimal Longitude,
    bool IsDefault);
