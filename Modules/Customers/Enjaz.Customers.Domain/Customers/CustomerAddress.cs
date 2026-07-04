namespace Enjaz.Customers.Domain.Customers;

public sealed class CustomerAddress
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }

    public CustomerProfile Customer { get; set; } = null!;

    public string? Label { get; set; }

    public string City { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string? BuildingNumber { get; set; }

    public string? Floor { get; set; }

    public string? Apartment { get; set; }

    public string? Landmark { get; set; }

    public string? FormattedAddress { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}
