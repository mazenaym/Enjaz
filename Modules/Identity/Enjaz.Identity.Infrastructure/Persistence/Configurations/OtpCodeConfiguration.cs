using Enjaz.Identity.Domain.Otp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Identity.Infrastructure.Persistence.Configurations;

public sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("otp_codes");

        builder.HasKey(otp => otp.Id);

        builder.Property(otp => otp.Id).HasColumnName("id");
        builder.Property(otp => otp.PhoneNumber).HasColumnName("phone_number").HasMaxLength(32).IsRequired();
        builder.Property(otp => otp.CodeHash).HasColumnName("code_hash").HasMaxLength(256).IsRequired();
        builder.Property(otp => otp.Purpose).HasColumnName("purpose").HasMaxLength(50).IsRequired();
        builder.Property(otp => otp.ExpiresAtUtc).HasColumnName("expires_at_utc");
        builder.Property(otp => otp.UsedAtUtc).HasColumnName("used_at_utc");
        builder.Property(otp => otp.AttemptCount).HasColumnName("attempt_count");
        builder.Property(otp => otp.MaxAttempts).HasColumnName("max_attempts").HasDefaultValue(5);
        builder.Property(otp => otp.CreatedAtUtc).HasColumnName("created_at_utc");

        builder.HasIndex(otp => new { otp.PhoneNumber, otp.Purpose, otp.CreatedAtUtc });
    }
}
