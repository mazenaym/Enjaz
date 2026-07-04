using Enjaz.Notifications.Domain.Notifications;

namespace Enjaz.Notifications.Infrastructure.Persistence.Configurations;

internal static class NotificationTemplateSeed
{
    private static readonly DateTime SeededAtUtc = new(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc);

    public static readonly NotificationTemplate[] Templates =
    [
        Template("10000000-0000-0000-0000-000000000801", NotificationTypes.JobCreated, "تم إنشاء الطلب", "تم إنشاء طلبك رقم {{jobNumber}} بنجاح."),
        Template("10000000-0000-0000-0000-000000000802", NotificationTypes.JobStatusChanged, "تحديث حالة الطلب", "تم تحديث حالة طلبك رقم {{jobNumber}} إلى {{status}}."),
        Template("10000000-0000-0000-0000-000000000803", NotificationTypes.TechnicianAssigned, "تم تعيين فني", "تم تعيين فني لطلبك رقم {{jobNumber}}."),
        Template("10000000-0000-0000-0000-000000000804", NotificationTypes.AssignmentAccepted, "تم قبول المهمة", "تم قبول المهمة رقم {{jobNumber}}."),
        Template("10000000-0000-0000-0000-000000000805", NotificationTypes.AssignmentRejected, "تم رفض المهمة", "تم رفض المهمة رقم {{jobNumber}}."),
        Template("10000000-0000-0000-0000-000000000806", NotificationTypes.JobCancelled, "تم إلغاء الطلب", "تم إلغاء طلبك رقم {{jobNumber}}."),
        Template("10000000-0000-0000-0000-000000000807", NotificationTypes.PaymentCheckoutCreated, "تم إنشاء رابط الدفع", "تم إنشاء رابط الدفع لطلبك رقم {{jobNumber}}."),
        Template("10000000-0000-0000-0000-000000000808", NotificationTypes.PaymentSucceeded, "تم الدفع بنجاح", "تم استلام دفعتك لطلب رقم {{jobNumber}} بمبلغ {{amount}} {{currency}}."),
        Template("10000000-0000-0000-0000-000000000809", NotificationTypes.PaymentFailed, "فشل الدفع", "تعذر إتمام الدفع لطلب رقم {{jobNumber}}."),
        Template("10000000-0000-0000-0000-000000000810", NotificationTypes.WalletUpdated, "تم تحديث المحفظة", "تم تحديث محفظتك بمبلغ {{amount}} {{currency}}."),
        Template("10000000-0000-0000-0000-000000000811", NotificationTypes.TechnicianEarningPending, "ربح قيد الانتظار", "لديك ربح قيد الانتظار بمبلغ {{amount}} {{currency}}."),
        Template("10000000-0000-0000-0000-000000000812", NotificationTypes.TechnicianEarningAvailable, "ربح متاح", "أصبح ربحك متاحا بمبلغ {{amount}} {{currency}}.")
    ];

    private static NotificationTemplate Template(string id, string type, string title, string body) => new()
    {
        Id = Guid.Parse(id),
        Type = type,
        Channel = NotificationChannels.InApp,
        Language = "ar",
        TitleTemplate = title,
        BodyTemplate = body,
        IsActive = true,
        CreatedAtUtc = SeededAtUtc
    };
}
