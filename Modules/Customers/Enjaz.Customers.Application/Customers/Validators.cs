using FluentValidation;

namespace Enjaz.Customers.Application.Customers;

public sealed class UpdateCustomerProfileRequestValidator : AbstractValidator<UpdateCustomerProfileRequest>
{
    public UpdateCustomerProfileRequestValidator()
    {
        RuleFor(request => request.FullName).NotEmpty().MaximumLength(200);
    }
}

public sealed class CustomerAddressRequestValidator : AbstractValidator<CustomerAddressRequest>
{
    public CustomerAddressRequestValidator()
    {
        RuleFor(request => request.City).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Area).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Street).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Latitude).InclusiveBetween(-90, 90);
        RuleFor(request => request.Longitude).InclusiveBetween(-180, 180);
    }
}
