namespace Enjaz.Wallets.Domain.Wallets;

public static class WalletOwnerTypes
{
    public const string Customer = "Customer";
    public const string Technician = "Technician";
    public const string Platform = "Platform";
    public const string Tax = "Tax";
    public const string ExternalPaymentProvider = "ExternalPaymentProvider";
}

public static class LedgerSourceModules
{
    public const string Payments = "Payments";
    public const string Jobs = "Jobs";
    public const string Refunds = "Refunds";
    public const string Payouts = "Payouts";
    public const string Admin = "Admin";
}

public static class LedgerTransactionTypes
{
    public const string PaymentCaptured = "PaymentCaptured";
    public const string TechnicianEarningPending = "TechnicianEarningPending";
    public const string PlatformCommissionEarned = "PlatformCommissionEarned";
    public const string VatLiabilityRecorded = "VatLiabilityRecorded";
    public const string RefundReserved = "RefundReserved";
    public const string RefundCompleted = "RefundCompleted";
    public const string PayoutReleased = "PayoutReleased";
    public const string TechnicianEarningReleased = "TechnicianEarningReleased";
    public const string AdminAdjustment = "AdminAdjustment";
}

public static class LedgerEntryDirections
{
    public const string Credit = "Credit";
    public const string Debit = "Debit";
}

public static class LedgerBalanceTypes
{
    public const string Available = "Available";
    public const string Pending = "Pending";
}

public static class PlatformEarningStatuses
{
    public const string Recorded = "Recorded";
    public const string Refunded = "Refunded";
    public const string PartiallyRefunded = "PartiallyRefunded";
}

public static class TechnicianEarningStatuses
{
    public const string Pending = "Pending";
    public const string Available = "Available";
    public const string PaidOut = "PaidOut";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";
}

public static class PayoutBatchStatuses
{
    public const string Draft = "Draft";
    public const string Processing = "Processing";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Cancelled = "Cancelled";
}

public static class PayoutBatchItemStatuses
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
