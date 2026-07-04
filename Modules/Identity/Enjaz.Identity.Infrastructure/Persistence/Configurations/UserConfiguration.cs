using Enjaz.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id).HasColumnName("id");
        builder.Property(user => user.PhoneNumber).HasColumnName("phone_number").HasMaxLength(32).IsRequired();
        builder.Property(user => user.FullName).HasColumnName("full_name").HasMaxLength(200);
        builder.Property(user => user.Email).HasColumnName("email").HasMaxLength(320);
        builder.Property(user => user.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(320);
        builder.Property(user => user.Username).HasColumnName("username").HasMaxLength(100);
        builder.Property(user => user.NormalizedUsername).HasColumnName("normalized_username").HasMaxLength(100);
        builder.Property(user => user.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
        builder.Property(user => user.IsEmailVerified).HasColumnName("is_email_verified").HasDefaultValue(false);
        builder.Property(user => user.IsPhoneVerified).HasColumnName("is_phone_verified").HasDefaultValue(false);
        builder.Property(user => user.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(user => user.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(user => user.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(user => user.LastLoginAtUtc).HasColumnName("last_login_at_utc");

        builder.HasIndex(user => user.PhoneNumber).IsUnique();
        builder.HasIndex(user => user.NormalizedEmail).IsUnique().HasFilter("normalized_email IS NOT NULL");
        builder.HasIndex(user => user.NormalizedUsername).IsUnique().HasFilter("normalized_username IS NOT NULL");
    }
}
