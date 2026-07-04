namespace Enjaz.Identity.Domain.Users;

public sealed class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string PhoneNumber { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public string? Username { get; set; }

    public string? NormalizedUsername { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public DateTime? LastLoginAtUtc { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
