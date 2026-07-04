using FluentValidation;

namespace Enjaz.Wallets.Application.Wallets;

public sealed class CreatePayoutBatchRequestValidator : AbstractValidator<CreatePayoutBatchRequest>
{
    public CreatePayoutBatchRequestValidator()
    {
        RuleFor(request => request.TechnicianIds).NotEmpty();
        RuleForEach(request => request.TechnicianIds).NotEmpty();
        RuleFor(request => request.Currency).MaximumLength(3);
    }
}
