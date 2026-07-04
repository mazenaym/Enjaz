using FluentValidation;

namespace Enjaz.Technicians.Application.Technicians;

public sealed class ApplyAsTechnicianRequestValidator : AbstractValidator<ApplyAsTechnicianRequest>
{
    public ApplyAsTechnicianRequestValidator()
    {
        RuleFor(request => request.FullName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.YearsOfExperience).GreaterThanOrEqualTo(0).When(request => request.YearsOfExperience.HasValue);
        RuleFor(request => request.Skills).NotEmpty();
        RuleForEach(request => request.Skills).SetValidator(new TechnicianSkillRequestValidator());
        RuleForEach(request => request.Documents).SetValidator(new TechnicianDocumentRequestValidator());
    }
}

public sealed class TechnicianSkillRequestValidator : AbstractValidator<TechnicianSkillRequest>
{
    public TechnicianSkillRequestValidator()
    {
        RuleFor(request => request.ServiceCategoryId).NotEmpty();
        RuleFor(request => request.ServiceId).NotEmpty();
        RuleFor(request => request.SkillLevel).MaximumLength(50);
    }
}

public sealed class TechnicianDocumentRequestValidator : AbstractValidator<TechnicianDocumentRequest>
{
    public TechnicianDocumentRequestValidator()
    {
        RuleFor(request => request.DocumentType).NotEmpty().MaximumLength(100);
        RuleFor(request => request.FileUrl).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.FileKey).MaximumLength(500);
    }
}

public sealed class UpdateTechnicianProfileRequestValidator : AbstractValidator<UpdateTechnicianProfileRequest>
{
    public UpdateTechnicianProfileRequestValidator()
    {
        RuleFor(request => request.YearsOfExperience).GreaterThanOrEqualTo(0).When(request => request.YearsOfExperience.HasValue);
        RuleFor(request => request.ProfileImageUrl).MaximumLength(1000);
        RuleFor(request => request.Email).EmailAddress().When(request => !string.IsNullOrWhiteSpace(request.Email));
    }
}

public sealed class UpdateTechnicianSkillsRequestValidator : AbstractValidator<UpdateTechnicianSkillsRequest>
{
    public UpdateTechnicianSkillsRequestValidator()
    {
        RuleFor(request => request.Skills).NotEmpty();
        RuleForEach(request => request.Skills).SetValidator(new TechnicianSkillRequestValidator());
        RuleFor(request => request.Skills)
            .Must(skills => skills.Select(skill => skill.ServiceId).Distinct().Count() == skills.Count)
            .WithMessage("Duplicate serviceId values are not allowed.");
    }
}

public sealed class RejectTechnicianRequestValidator : AbstractValidator<RejectTechnicianRequest>
{
    public RejectTechnicianRequestValidator()
    {
        RuleFor(request => request.Reason).NotEmpty().MaximumLength(1000);
    }
}
