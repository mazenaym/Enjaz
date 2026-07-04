using FluentValidation;

namespace Enjaz.Catalog.Application.Catalog;

public sealed class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(request => request.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Slug).NotEmpty().MaximumLength(200).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$");
        RuleFor(request => request.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(request => request.IconUrl).MaximumLength(1000);
    }
}

public sealed class ServiceRequestValidator : AbstractValidator<ServiceRequest>
{
    public ServiceRequestValidator()
    {
        RuleFor(request => request.CategoryId).NotEmpty();
        RuleFor(request => request.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Slug).NotEmpty().MaximumLength(200).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$");
        RuleFor(request => request.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(request => request.IconUrl).MaximumLength(1000);
    }
}

public sealed class ServiceTierRequestValidator : AbstractValidator<ServiceTierRequest>
{
    private static readonly string[] AllowedNames = ["Simple", "Standard", "Complex"];

    public ServiceTierRequestValidator()
    {
        RuleFor(request => request.ServiceId).NotEmpty();
        RuleFor(request => request.Name).NotEmpty().Must(name => AllowedNames.Contains(name)).WithMessage("Tier name is invalid.");
        RuleFor(request => request.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
