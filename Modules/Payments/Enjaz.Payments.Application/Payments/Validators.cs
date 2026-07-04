using FluentValidation;

namespace Enjaz.Payments.Application.Payments;

public sealed class CreateCheckoutRequestValidator : AbstractValidator<CreateCheckoutRequest>
{
    public CreateCheckoutRequestValidator()
    {
        RuleFor(request => request.JobId).NotEmpty();
    }
}

public sealed class FakePaymentFailRequestValidator : AbstractValidator<FakePaymentFailRequest>
{
    public FakePaymentFailRequestValidator()
    {
        RuleFor(request => request.Reason).MaximumLength(500);
    }
}
