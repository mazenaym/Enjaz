using Enjaz.SharedKernel.Auth;
using Enjaz.SharedKernel.Results;
using Enjaz.Technicians.Domain.Technicians;

namespace Enjaz.Technicians.Application.Technicians;

public sealed class TechnicianService(
    ITechniciansRepository repository,
    ICurrentUserContext currentUserContext,
    IIdentityUserService identityUserService) : ITechnicianService
{
    private const string TechnicianRole = "Technician";

    public async Task<Result<TechnicianProfileResponse>> ApplyAsync(ApplyAsTechnicianRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId == Guid.Empty)
        {
            return Result.Failure<TechnicianProfileResponse>("unauthenticated", "Authentication is required.");
        }

        if (await repository.ExistsForUserAsync(currentUserContext.UserId, cancellationToken))
        {
            return Result.Failure<TechnicianProfileResponse>("technician_application_exists", "Technician profile already exists.");
        }

        var userResult = await identityUserService.GetUserInfoAsync(currentUserContext.UserId, cancellationToken);
        if (userResult.IsFailure || userResult.Value is null)
        {
            return Result.Failure<TechnicianProfileResponse>(userResult.ErrorCode!, userResult.ErrorMessage!);
        }

        if (!userResult.Value.IsPhoneVerified)
        {
            return Result.Failure<TechnicianProfileResponse>("phone_not_verified", "Phone number must be verified before applying as a technician.");
        }

        if (!userResult.Value.Roles.Any(role => string.Equals(role, TechnicianRole, StringComparison.OrdinalIgnoreCase)))
        {
            var roleResult = await identityUserService.AssignRoleAsync(currentUserContext.UserId, TechnicianRole, cancellationToken);
            if (roleResult.IsFailure)
            {
                return Result.Failure<TechnicianProfileResponse>(roleResult.ErrorCode!, roleResult.ErrorMessage!);
            }
        }

        var profile = new TechnicianProfile
        {
            UserId = currentUserContext.UserId,
            FullName = request.FullName.Trim(),
            PhoneNumber = userResult.Value.PhoneNumber,
            Email = TrimOptional(request.Email) ?? userResult.Value.Email,
            NationalId = TrimOptional(request.NationalId),
            Bio = TrimOptional(request.Bio),
            YearsOfExperience = request.YearsOfExperience,
            ProfileImageUrl = TrimOptional(request.ProfileImageUrl),
            Status = TechnicianStatuses.Pending,
            AvailabilityStatus = TechnicianAvailabilityStatuses.Offline,
            CreatedAtUtc = DateTime.UtcNow,
            Skills = request.Skills.Select(MapSkill).ToList(),
            Documents = (request.Documents ?? []).Select(MapDocument).ToList()
        };

        await repository.AddProfileAsync(profile, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(profile));
    }

    public async Task<Result<TechnicianProfileResponse>> GetMyProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentProfileAsync(cancellationToken);
        return profile is null
            ? Result.Failure<TechnicianProfileResponse>("technician_profile_not_found", "Technician profile was not found.")
            : Result.Success(Map(profile));
    }

    public async Task<Result<TechnicianProfileResponse>> UpdateMyProfileAsync(UpdateTechnicianProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentProfileAsync(cancellationToken);
        if (profile is null)
        {
            return Result.Failure<TechnicianProfileResponse>("technician_profile_not_found", "Technician profile was not found.");
        }

        profile.Bio = TrimOptional(request.Bio);
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.ProfileImageUrl = TrimOptional(request.ProfileImageUrl);
        profile.Email = TrimOptional(request.Email);
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(profile));
    }

    public async Task<Result<TechnicianDocumentResponse>> AddDocumentAsync(TechnicianDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure<TechnicianDocumentResponse>("technician_profile_not_found", "Technician profile was not found.");
        }

        var document = MapDocument(request);
        document.TechnicianId = profile.Id;
        await repository.AddDocumentAsync(document, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(document));
    }

    public async Task<Result<IReadOnlyCollection<TechnicianDocumentResponse>>> GetMyDocumentsAsync(CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentProfileAsync(cancellationToken);
        if (profile is null)
        {
            return Result.Failure<IReadOnlyCollection<TechnicianDocumentResponse>>("technician_profile_not_found", "Technician profile was not found.");
        }

        return Result.Success<IReadOnlyCollection<TechnicianDocumentResponse>>(profile.Documents.Select(Map).ToArray());
    }

    public async Task<Result<TechnicianProfileResponse>> UpdateSkillsAsync(UpdateTechnicianSkillsRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await GetCurrentProfileAsync(cancellationToken);
        if (profile is null)
        {
            return Result.Failure<TechnicianProfileResponse>("technician_profile_not_found", "Technician profile was not found.");
        }

        if (request.Skills.Select(skill => skill.ServiceId).Distinct().Count() != request.Skills.Count)
        {
            return Result.Failure<TechnicianProfileResponse>("duplicate_skill", "Duplicate serviceId values are not allowed.");
        }

        repository.RemoveSkills(profile.Skills.ToArray());
        profile.Skills = request.Skills.Select(MapSkill).ToList();
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(profile));
    }

    public async Task<Result> GoOnlineAsync(CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("technician_profile_not_found", "Technician profile was not found.");
        }

        if (profile.Status != TechnicianStatuses.Approved)
        {
            return Result.Failure("technician_not_approved", "Only approved technicians can go online.");
        }

        var history = SetAvailability(profile, TechnicianAvailabilityStatuses.Online, currentUserContext.UserId);
        if (history is not null)
        {
            await repository.AddAvailabilityHistoryAsync(history, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> GoOfflineAsync(CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByUserIdAsync(currentUserContext.UserId, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("technician_profile_not_found", "Technician profile was not found.");
        }

        var history = SetAvailability(profile, TechnicianAvailabilityStatuses.Offline, currentUserContext.UserId);
        if (history is not null)
        {
            await repository.AddAvailabilityHistoryAsync(history, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<TechnicianProfileResponse>>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await repository.GetPendingAsync(cancellationToken);
        return Result.Success<IReadOnlyCollection<TechnicianProfileResponse>>(profiles.Select(Map).ToArray());
    }

    public async Task<Result<TechnicianProfileResponse>> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetWithDetailsAsync(id, cancellationToken);
        return profile is null
            ? Result.Failure<TechnicianProfileResponse>("technician_profile_not_found", "Technician profile was not found.")
            : Result.Success(Map(profile));
    }

    public async Task<Result> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetWithDetailsAsync(id, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("technician_profile_not_found", "Technician profile was not found.");
        }

        profile.Status = TechnicianStatuses.Approved;
        profile.ApprovedAtUtc = DateTime.UtcNow;
        profile.RejectedAtUtc = null;
        profile.RejectionReason = null;
        profile.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RejectAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetWithDetailsAsync(id, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("technician_profile_not_found", "Technician profile was not found.");
        }

        profile.Status = TechnicianStatuses.Rejected;
        profile.RejectedAtUtc = DateTime.UtcNow;
        profile.RejectionReason = reason.Trim();
        var history = SetAvailability(profile, TechnicianAvailabilityStatuses.Offline, currentUserContext.UserId);
        if (history is not null)
        {
            await repository.AddAvailabilityHistoryAsync(history, cancellationToken);
        }

        profile.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SuspendAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetWithDetailsAsync(id, cancellationToken);
        if (profile is null)
        {
            return Result.Failure("technician_profile_not_found", "Technician profile was not found.");
        }

        profile.Status = TechnicianStatuses.Suspended;
        profile.RejectionReason = reason.Trim();
        var history = SetAvailability(profile, TechnicianAvailabilityStatuses.Offline, currentUserContext.UserId);
        if (history is not null)
        {
            await repository.AddAvailabilityHistoryAsync(history, cancellationToken);
        }

        profile.UpdatedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ApproveDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDocumentAsync(documentId, cancellationToken);
        if (document is null)
        {
            return Result.Failure("technician_document_not_found", "Technician document was not found.");
        }

        document.Status = TechnicianDocumentStatuses.Approved;
        document.RejectionReason = null;
        document.ReviewedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RejectDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDocumentAsync(documentId, cancellationToken);
        if (document is null)
        {
            return Result.Failure("technician_document_not_found", "Technician document was not found.");
        }

        document.Status = TechnicianDocumentStatuses.Rejected;
        document.RejectionReason = reason.Trim();
        document.ReviewedAtUtc = DateTime.UtcNow;
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<TechnicianProfile?> GetCurrentProfileAsync(CancellationToken cancellationToken)
    {
        return await repository.GetWithDetailsByUserIdAsync(currentUserContext.UserId, cancellationToken);
    }

    private static TechnicianAvailabilityHistory? SetAvailability(TechnicianProfile profile, string toStatus, Guid? changedByUserId)
    {
        if (profile.AvailabilityStatus == toStatus)
        {
            return null;
        }

        var history = new TechnicianAvailabilityHistory
        {
            TechnicianId = profile.Id,
            FromStatus = profile.AvailabilityStatus,
            ToStatus = toStatus,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedByUserId = changedByUserId
        };
        profile.AvailabilityStatus = toStatus;
        profile.UpdatedAtUtc = DateTime.UtcNow;

        return history;
    }

    private static TechnicianSkill MapSkill(TechnicianSkillRequest request)
    {
        return new TechnicianSkill
        {
            ServiceCategoryId = request.ServiceCategoryId,
            ServiceId = request.ServiceId,
            SkillLevel = TrimOptional(request.SkillLevel),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static TechnicianDocument MapDocument(TechnicianDocumentRequest request)
    {
        return new TechnicianDocument
        {
            DocumentType = request.DocumentType.Trim(),
            FileUrl = request.FileUrl.Trim(),
            FileKey = TrimOptional(request.FileKey),
            Status = TechnicianDocumentStatuses.Pending,
            UploadedAtUtc = DateTime.UtcNow
        };
    }

    private static TechnicianProfileResponse Map(TechnicianProfile profile)
    {
        return new TechnicianProfileResponse(
            profile.Id,
            profile.UserId,
            profile.FullName,
            profile.PhoneNumber,
            profile.Email,
            profile.NationalId,
            profile.ProfileImageUrl,
            profile.Bio,
            profile.YearsOfExperience,
            profile.Status,
            profile.AvailabilityStatus,
            profile.AverageRating,
            profile.TotalReviews,
            profile.CreatedAtUtc,
            profile.ApprovedAtUtc,
            profile.RejectedAtUtc,
            profile.RejectionReason,
            profile.Skills.Select(Map).ToArray(),
            profile.Documents.Select(Map).ToArray());
    }

    private static TechnicianSkillResponse Map(TechnicianSkill skill)
    {
        return new TechnicianSkillResponse(skill.Id, skill.ServiceCategoryId, skill.ServiceId, skill.SkillLevel);
    }

    private static TechnicianDocumentResponse Map(TechnicianDocument document)
    {
        return new TechnicianDocumentResponse(document.Id, document.TechnicianId, document.DocumentType, document.FileUrl, document.FileKey, document.Status, document.RejectionReason, document.UploadedAtUtc, document.ReviewedAtUtc);
    }

    private static string? TrimOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
