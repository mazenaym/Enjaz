using Enjaz.Jobs.Domain.Jobs;
using FluentValidation;

namespace Enjaz.Jobs.Application.Jobs;

public sealed class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(request => request.CustomerAddressId).NotEmpty();
        RuleFor(request => request.ServiceCategoryId).NotEmpty();
        RuleFor(request => request.ServiceId).NotEmpty();
        RuleFor(request => request.PriceSnapshotId).NotEmpty();
        RuleFor(request => request.Title).MaximumLength(200);
        RuleFor(request => request.Description).NotEmpty().MinimumLength(10).MaximumLength(2000);
        RuleFor(request => request.ScheduledAtUtc)
            .Must(value => value is null || value.Value > DateTime.UtcNow)
            .WithMessage("Scheduled time cannot be in the past.");
        RuleFor(request => request)
            .Must(request => request.PreferredTimeWindowStartUtc is null || request.PreferredTimeWindowEndUtc is null || request.PreferredTimeWindowStartUtc < request.PreferredTimeWindowEndUtc)
            .WithMessage("Preferred time window start must be before end.");
        RuleFor(request => request.Media).Must(media => media is null || media.Count <= 10).WithMessage("Media cannot exceed 10 items.");
        RuleForEach(request => request.Media).SetValidator(new JobMediaRequestValidator());
    }
}

public sealed class JobMediaRequestValidator : AbstractValidator<JobMediaRequest>
{
    public JobMediaRequestValidator()
    {
        RuleFor(request => request.MediaType).NotEmpty().Must(JobMediaTypes.All.Contains).WithMessage("Invalid media type.");
        RuleFor(request => request.FileUrl).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.FileKey).MaximumLength(500);
        RuleFor(request => request.Caption).MaximumLength(500);
    }
}

public sealed class CancelJobRequestValidator : AbstractValidator<CancelJobRequest>
{
    public CancelJobRequestValidator()
    {
        RuleFor(request => request.Reason).NotEmpty().MaximumLength(500);
    }
}

public sealed class AdminUpdateJobStatusRequestValidator : AbstractValidator<AdminUpdateJobStatusRequest>
{
    public AdminUpdateJobStatusRequestValidator()
    {
        RuleFor(request => request.Status).NotEmpty().Must(JobStatuses.All.Contains).WithMessage("Invalid job status.");
        RuleFor(request => request.Reason).MaximumLength(500);
    }
}

public sealed class AdminAddJobNoteRequestValidator : AbstractValidator<AdminAddJobNoteRequest>
{
    public AdminAddJobNoteRequestValidator()
    {
        RuleFor(request => request.Text).NotEmpty().MaximumLength(2000);
    }
}

public sealed class AssignTechnicianRequestValidator : AbstractValidator<AssignTechnicianRequest>
{
    public AssignTechnicianRequestValidator()
    {
        RuleFor(request => request.TechnicianId).NotEmpty();
        RuleFor(request => request.ExpiresInMinutes).InclusiveBetween(5, 120);
    }
}

public sealed class RejectAssignmentRequestValidator : AbstractValidator<RejectAssignmentRequest>
{
    public RejectAssignmentRequestValidator()
    {
        RuleFor(request => request.Reason).NotEmpty().MaximumLength(500);
    }
}
