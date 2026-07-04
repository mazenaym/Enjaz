namespace Enjaz.Payments.Domain.Payments;

public static class PaymentProviders
{
    public const string Fake = "Fake";
    public const string Paymob = "Paymob";
}

public static class PaymentStatuses
{
    public const string Created = "Created";
    public const string Pending = "Pending";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";

    public static readonly IReadOnlySet<string> Terminal = new HashSet<string> { Succeeded, Failed, Cancelled, Refunded };
    public static readonly IReadOnlySet<string> Active = new HashSet<string> { Created, Pending };
}

public static class PaymentTransactionTypes
{
    public const string CheckoutCreated = "CheckoutCreated";
    public const string PaymentSucceeded = "PaymentSucceeded";
    public const string PaymentFailed = "PaymentFailed";
    public const string RefundCreated = "RefundCreated";
    public const string RefundSucceeded = "RefundSucceeded";
    public const string RefundFailed = "RefundFailed";
}

public static class RefundRequestStatuses
{
    public const string Requested = "Requested";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Processing = "Processing";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
}
