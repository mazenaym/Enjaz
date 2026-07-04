using Enjaz.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enjaz.Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    private static readonly Role[] Roles =
    [
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin", NormalizedName = "ADMIN" },
        new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Customer", NormalizedName = "CUSTOMER" },
        new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Technician", NormalizedName = "TECHNICIAN" },
        new() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Support", NormalizedName = "SUPPORT" },
        new() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Finance", NormalizedName = "FINANCE" },
        new() { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Operations", NormalizedName = "OPERATIONS" }
    ];

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id).HasColumnName("id");
        builder.Property(role => role.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(role => role.NormalizedName).HasColumnName("normalized_name").HasMaxLength(100).IsRequired();

        builder.HasIndex(role => role.Name).IsUnique();
        builder.HasIndex(role => role.NormalizedName).IsUnique();

        builder.HasData(Roles);
    }
}
