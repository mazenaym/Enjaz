using FluentValidation;

namespace Enjaz.Maps.Application.Maps;

public sealed class ServiceZoneRequestValidator : AbstractValidator<ServiceZoneRequest>
{
    public ServiceZoneRequestValidator()
    {
        RuleFor(request => request.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(request => request.NameEn).MaximumLength(200);
        RuleFor(request => request.City).NotEmpty().MaximumLength(120);
        RuleFor(request => request.Area).MaximumLength(120);
        RuleFor(request => request.Slug).NotEmpty().MaximumLength(200).Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$");
        RuleFor(request => request.Polygon)
            .NotNull()
            .Must(HaveEnoughUniquePoints)
            .WithMessage("Polygon must contain at least 3 unique points.");
        RuleForEach(request => request.Polygon).SetValidator(new LocationPointRequestValidator());
    }

    private static bool HaveEnoughUniquePoints(IReadOnlyCollection<LocationPointRequest>? points)
    {
        return points is not null && points.Select(point => (point.Lat, point.Lng)).Distinct().Count() >= 3;
    }
}

public sealed class LocationPointRequestValidator : AbstractValidator<LocationPointRequest>
{
    public LocationPointRequestValidator()
    {
        RuleFor(point => point.Lat).InclusiveBetween(-90m, 90m);
        RuleFor(point => point.Lng).InclusiveBetween(-180m, 180m);
    }
}

public sealed class UpdateTechnicianLocationRequestValidator : AbstractValidator<UpdateTechnicianLocationRequest>
{
    public UpdateTechnicianLocationRequestValidator()
    {
        RuleFor(request => request.Latitude).InclusiveBetween(-90m, 90m);
        RuleFor(request => request.Longitude).InclusiveBetween(-180m, 180m);
        RuleFor(request => request.AccuracyMeters).GreaterThanOrEqualTo(0).When(request => request.AccuracyMeters.HasValue);
        RuleFor(request => request.Heading).InclusiveBetween(0m, 360m).When(request => request.Heading.HasValue);
        RuleFor(request => request.SpeedMetersPerSecond).GreaterThanOrEqualTo(0).When(request => request.SpeedMetersPerSecond.HasValue);
        RuleFor(request => request.Source).MaximumLength(50);
    }
}
