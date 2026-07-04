namespace Enjaz.Notifications.Domain.Notifications;

public static class NotificationChannels
{
    public const string InApp = "InApp";
    public const string Sms = "SMS";
    public const string Push = "Push";
    public const string Email = "Email";
}

public static class NotificationTypes
{
    public const string JobCreated = "JobCreated";
    public const string JobStatusChanged = "JobStatusChanged";
    public const string TechnicianAssigned = "TechnicianAssigned";
    public const string AssignmentAccepted = "AssignmentAccepted";
    public const string AssignmentRejected = "AssignmentRejected";
    public const string JobCancelled = "JobCancelled";
    public const string PaymentCheckoutCreated = "PaymentCheckoutCreated";
    public const string PaymentSucceeded = "PaymentSucceeded";
    public const string PaymentFailed = "PaymentFailed";
    public const string WalletUpdated = "WalletUpdated";
    public const string TechnicianEarningPending = "TechnicianEarningPending";
    public const string TechnicianEarningAvailable = "TechnicianEarningAvailable";
    public const string General = "General";
}

public static class NotificationProviders
{
    public const string Fake = "Fake";
    public const string VictoryLink = "VictoryLink";
    public const string Cequens = "Cequens";
    public const string Firebase = "Firebase";
    public const string Expo = "Expo";
    public const string Email = "Email";
}

public static class NotificationDeliveryStatuses
{
    public const string Pending = "Pending";
    public const string Sent = "Sent";
    public const string Failed = "Failed";
    public const string Retrying = "Retrying";
}

public static class PushPlatforms
{
    public const string Android = "Android";
    public const string Ios = "iOS";
    public const string Web = "Web";
}
