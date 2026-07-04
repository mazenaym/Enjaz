namespace Enjaz.Jobs.Domain.Jobs;

public static class JobStatuses
{
    public const string Draft = "Draft";
    public const string PendingPricing = "PendingPricing";
    public const string PendingInspectionPricing = "PendingInspectionPricing";
    public const string WaitingForPayment = "WaitingForPayment";
    public const string Paid = "Paid";
    public const string SearchingTechnician = "SearchingTechnician";
    public const string WaitingForManualAssignment = "WaitingForManualAssignment";
    public const string TechnicianAssigned = "TechnicianAssigned";
    public const string TechnicianAccepted = "TechnicianAccepted";
    public const string TechnicianOnWay = "TechnicianOnWay";
    public const string Arrived = "Arrived";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
    public const string Disputed = "Disputed";

    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Draft, PendingPricing, PendingInspectionPricing, WaitingForPayment, Paid,
        SearchingTechnician, WaitingForManualAssignment, TechnicianAssigned,
        TechnicianAccepted, TechnicianOnWay, Arrived, InProgress, Completed,
        Cancelled, Disputed
    };
}

public static class JobMediaTypes
{
    public const string Image = "Image";
    public const string Video = "Video";
    public const string Audio = "Audio";
    public const string Document = "Document";

    public static readonly IReadOnlySet<string> All = new HashSet<string> { Image, Video, Audio, Document };
}

public static class JobNoteAuthorRoles
{
    public const string Customer = "Customer";
    public const string Technician = "Technician";
    public const string Admin = "Admin";
    public const string System = "System";
}

public static class JobNoteTypes
{
    public const string General = "General";
    public const string Internal = "Internal";
    public const string Cancellation = "Cancellation";
    public const string Pricing = "Pricing";
    public const string Assignment = "Assignment";
}

public static class JobAssignmentStatuses
{
    public const string Offered = "Offered";
    public const string Accepted = "Accepted";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
    public const string Expired = "Expired";
}
