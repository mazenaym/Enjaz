using FluentValidation;

namespace Enjaz.AI.Application.AI;

public sealed class ClassifyIssueRequestValidator : AbstractValidator<ClassifyIssueRequest>
{
    public ClassifyIssueRequestValidator()
    {
        RuleFor(request => request.ServiceCategoryId).NotEmpty();
        RuleFor(request => request.ServiceId).NotEmpty();
        RuleFor(request => request.Description).NotEmpty().MinimumLength(10).MaximumLength(4000);
    }
}
