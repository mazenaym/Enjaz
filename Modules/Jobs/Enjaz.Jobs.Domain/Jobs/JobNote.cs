namespace Enjaz.Jobs.Domain.Jobs;

public sealed class JobNote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string AuthorRole { get; set; } = JobNoteAuthorRoles.System;
    public string NoteType { get; set; } = JobNoteTypes.General;
    public string Text { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
