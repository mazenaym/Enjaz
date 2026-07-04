using Enjaz.Support.Domain.Support;
using FluentValidation;

namespace Enjaz.Support.Application.Support;

public sealed class CreateSupportTicketRequestValidator : AbstractValidator<CreateSupportTicketRequest>
{
    public CreateSupportTicketRequestValidator()
    {
        RuleFor(request => request.Category).Must(SupportTicketCategories.All.Contains);
        RuleFor(request => request.Priority).Must(SupportTicketPriorities.All.Contains);
        RuleFor(request => request.Subject).NotEmpty().MaximumLength(300);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(4000);
    }
}

public sealed class AddTicketMessageRequestValidator : AbstractValidator<AddTicketMessageRequest>
{
    public AddTicketMessageRequestValidator() => RuleFor(request => request.Message).NotEmpty().MaximumLength(4000);
}

public sealed class AdminUpdateTicketStatusRequestValidator : AbstractValidator<AdminUpdateTicketStatusRequest>
{
    public AdminUpdateTicketStatusRequestValidator() => RuleFor(request => request.Status).Must(SupportTicketStatuses.All.Contains);
}

public sealed class OpenDisputeRequestValidator : AbstractValidator<OpenDisputeRequest>
{
    public OpenDisputeRequestValidator() => RuleFor(request => request.Reason).NotEmpty().MaximumLength(2000);
}

public sealed class ResolveDisputeRequestValidator : AbstractValidator<ResolveDisputeRequest>
{
    public ResolveDisputeRequestValidator() => RuleFor(request => request.Resolution).NotEmpty().MaximumLength(2000);
}
