using Enjaz.Identity.Domain.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Identity.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Id).HasColumnName("id");
        builder.Property(token => token.UserId).HasColumnName("user_id");
        builder.Property(token => token.TokenHash).HasColumnName("token_hash").HasMaxLength(256).IsRequired();
        builder.Property(token => token.ExpiresAtUtc).HasColumnName("expires_at_utc");
        builder.Property(token => token.RevokedAtUtc).HasColumnName("revoked_at_utc");
        builder.Property(token => token.ReplacedByTokenHash).HasColumnName("replaced_by_token_hash").HasMaxLength(256);
        builder.Property(token => token.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(token => token.CreatedByIp).HasColumnName("created_by_ip").HasMaxLength(64);

        builder.HasIndex(token => token.TokenHash).IsUnique();

        builder
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId);
    }
}
