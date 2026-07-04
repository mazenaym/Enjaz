namespace Enjaz.Technicians.Application.Technicians;

public sealed record TechnicianSkillRequest(Guid ServiceCategoryId, Guid ServiceId, string? SkillLevel);

public sealed record TechnicianDocumentRequest(string DocumentType, string FileUrl, string? FileKey);

public sealed record ApplyAsTechnicianRequest(
    string FullName,
    string? Email,
    string? NationalId,
    string? Bio,
    int? YearsOfExperience,
    string? ProfileImageUrl,
    IReadOnlyCollection<TechnicianSkillRequest> Skills,
    IReadOnlyCollection<TechnicianDocumentRequest>? Documents);

public sealed record UpdateTechnicianProfileRequest(string? Bio, int? YearsOfExperience, string? ProfileImageUrl, string? Email);

public sealed record UpdateTechnicianSkillsRequest(IReadOnlyCollection<TechnicianSkillRequest> Skills);

public sealed record RejectTechnicianRequest(string Reason);

public sealed record TechnicianProfileResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? NationalId,
    string? ProfileImageUrl,
    string? Bio,
    int? YearsOfExperience,
    string Status,
    string AvailabilityStatus,
    decimal AverageRating,
    int TotalReviews,
    DateTime CreatedAtUtc,
    DateTime? ApprovedAtUtc,
    DateTime? RejectedAtUtc,
    string? RejectionReason,
    IReadOnlyCollection<TechnicianSkillResponse> Skills,
    IReadOnlyCollection<TechnicianDocumentResponse> Documents);

public sealed record TechnicianSkillResponse(Guid Id, Guid ServiceCategoryId, Guid ServiceId, string? SkillLevel);

public sealed record TechnicianDocumentResponse(Guid Id, Guid TechnicianId, string DocumentType, string FileUrl, string? FileKey, string Status, string? RejectionReason, DateTime UploadedAtUtc, DateTime? ReviewedAtUtc);
