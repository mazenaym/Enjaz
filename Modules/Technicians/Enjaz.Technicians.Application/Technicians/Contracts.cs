using Enjaz.SharedKernel.Results;
using Enjaz.Technicians.Domain.Technicians;

namespace Enjaz.Technicians.Application.Technicians;

public interface ITechnicianService
{
    Task<Result<TechnicianProfileResponse>> ApplyAsync(ApplyAsTechnicianRequest request, CancellationToken cancellationToken = default);
    Task<Result<TechnicianProfileResponse>> GetMyProfileAsync(CancellationToken cancellationToken = default);
    Task<Result<TechnicianProfileResponse>> UpdateMyProfileAsync(UpdateTechnicianProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result<TechnicianDocumentResponse>> AddDocumentAsync(TechnicianDocumentRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<TechnicianDocumentResponse>>> GetMyDocumentsAsync(CancellationToken cancellationToken = default);
    Task<Result<TechnicianProfileResponse>> UpdateSkillsAsync(UpdateTechnicianSkillsRequest request, CancellationToken cancellationToken = default);
    Task<Result> GoOnlineAsync(CancellationToken cancellationToken = default);
    Task<Result> GoOfflineAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<TechnicianProfileResponse>>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<Result<TechnicianProfileResponse>> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> RejectAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> SuspendAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task<Result> ApproveDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<Result> RejectDocumentAsync(Guid documentId, string reason, CancellationToken cancellationToken = default);
}

public interface ITechniciansRepository
{
    Task<TechnicianProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TechnicianProfile?> GetWithDetailsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TechnicianProfile?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TechnicianProfile>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<TechnicianDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddProfileAsync(TechnicianProfile profile, CancellationToken cancellationToken = default);
    Task AddDocumentAsync(TechnicianDocument document, CancellationToken cancellationToken = default);
    Task AddAvailabilityHistoryAsync(TechnicianAvailabilityHistory history, CancellationToken cancellationToken = default);
    void RemoveSkills(IReadOnlyCollection<TechnicianSkill> skills);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
