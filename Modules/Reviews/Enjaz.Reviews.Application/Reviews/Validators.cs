using FluentValidation;

namespace Enjaz.Reviews.Application.Reviews;

public sealed class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(request => request.Rating).InclusiveBetween(1, 5);
        RuleFor(request => request.Comment).MaximumLength(2000);
    }
}
