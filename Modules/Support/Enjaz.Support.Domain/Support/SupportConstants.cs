namespace Enjaz.Support.Domain.Support;

public static class SupportTicketCategories
{
    public const string General = "General";
    public const string Payment = "Payment";
    public const string JobIssue = "JobIssue";
    public const string TechnicianComplaint = "TechnicianComplaint";
    public const string RefundRequest = "RefundRequest";
    public const string TechnicalProblem = "TechnicalProblem";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { General, Payment, JobIssue, TechnicianComplaint, RefundRequest, TechnicalProblem };
}

public static class SupportTicketPriorities
{
    public const string Low = "Low";
    public const string Normal = "Normal";
    public const string High = "High";
    public const string Urgent = "Urgent";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Low, Normal, High, Urgent };
}

public static class SupportTicketStatuses
{
    public const string Open = "Open";
    public const string InProgress = "InProgress";
    public const string Resolved = "Resolved";
    public const string Closed = "Closed";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Open, InProgress, Resolved, Closed };
}

public static class SupportSenderRoles
{
    public const string Customer = "Customer";
    public const string Technician = "Technician";
    public const string Admin = "Admin";
    public const string System = "System";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Customer, Technician, Admin, System };
}

public static class JobDisputeStatuses
{
    public const string Open = "Open";
    public const string UnderReview = "UnderReview";
    public const string Resolved = "Resolved";
    public const string Rejected = "Rejected";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Open, UnderReview, Resolved, Rejected };
}
