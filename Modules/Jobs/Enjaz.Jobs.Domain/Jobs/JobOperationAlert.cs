namespace Enjaz.Jobs.Domain.Jobs;

public sealed class JobOperationAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAtUtc { get; set; }
}

public static class JobOperationAlertTypes
{
    public const string AssignmentNoResponse = "AssignmentNoResponse";
    public const string TechnicianOnWayLate = "TechnicianOnWayLate";
    public const string InProgressTooLong = "InProgressTooLong";
}
