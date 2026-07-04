using Enjaz.AI.Domain.AI;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.AI.Infrastructure.Persistence;

public sealed class AiDbContext : DbContext
{
    public AiDbContext(DbContextOptions<AiDbContext> options)
        : base(options)
    {
    }

    public DbSet<AiRequest> AiRequests => Set<AiRequest>();

    public DbSet<AiClassification> AiClassifications => Set<AiClassification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ai");
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
