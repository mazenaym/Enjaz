namespace Enjaz.Calls.Application.Calls;

public sealed record CallSessionResponse(Guid Id, Guid JobId, Guid CustomerUserId, Guid TechnicianUserId, Guid TechnicianId, Guid InitiatedByUserId, string Provider, string? ProviderCallId, string? MaskedNumber, string Status, DateTime? StartedAtUtc, DateTime? EndedAtUtc, int? DurationSeconds, string? RecordingUrl, DateTime CreatedAtUtc);
public sealed record CallWebhookLogResponse(Guid Id, string Provider, string? ProviderCallId, string? EventType, string RawPayloadJson, string? HeadersJson, bool IsProcessed, string? ProcessingError, DateTime ReceivedAtUtc, DateTime? ProcessedAtUtc);
public sealed record CallJobLookupResult(Guid JobId, Guid CustomerUserId, Guid? AssignedTechnicianId, Guid? AssignedTechnicianUserId, string Status);
public sealed record CreateMaskedCallRequest(Guid JobId, Guid CustomerUserId, Guid TechnicianUserId, string CustomerPhoneNumber, string TechnicianPhoneNumber, Guid InitiatedByUserId);
public sealed record CreateMaskedCallResult(string Provider, string ProviderCallId, string MaskedNumber, string Status);
public sealed record CallWebhookProcessRequest(string RawPayloadJson, IReadOnlyDictionary<string, string?> Headers);
