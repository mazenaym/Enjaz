using FluentValidation;

namespace Enjaz.Notifications.Application.Notifications;

public sealed class UpdateNotificationPreferenceRequestValidator : AbstractValidator<UpdateNotificationPreferenceRequest>
{
    public UpdateNotificationPreferenceRequestValidator()
    {
        RuleFor(request => request.Preferences).NotEmpty();
        RuleForEach(request => request.Preferences).ChildRules(item =>
        {
            item.RuleFor(preference => preference.Channel).NotEmpty().MaximumLength(20);
        });
    }
}

public sealed class RegisterDeviceTokenRequestValidator : AbstractValidator<RegisterDeviceTokenRequest>
{
    public RegisterDeviceTokenRequestValidator()
    {
        RuleFor(request => request.Token).NotEmpty().MaximumLength(500);
        RuleFor(request => request.Platform).MaximumLength(20);
        RuleFor(request => request.Provider).MaximumLength(30);
        RuleFor(request => request.DeviceId).MaximumLength(120);
    }
}

public sealed class NotificationTemplateRequestValidator : AbstractValidator<NotificationTemplateRequest>
{
    public NotificationTemplateRequestValidator()
    {
        RuleFor(request => request.Type).NotEmpty().MaximumLength(80);
        RuleFor(request => request.Channel).NotEmpty().MaximumLength(20);
        RuleFor(request => request.Language).MaximumLength(10);
        RuleFor(request => request.TitleTemplate).NotEmpty().MaximumLength(200);
        RuleFor(request => request.BodyTemplate).NotEmpty().MaximumLength(2000);
    }
}

public sealed class AdminTestNotificationRequestValidator : AbstractValidator<AdminTestNotificationRequest>
{
    public AdminTestNotificationRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
        RuleFor(request => request.Type).NotEmpty().MaximumLength(80);
        RuleFor(request => request.Title).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Body).NotEmpty().MaximumLength(2000);
    }
}
