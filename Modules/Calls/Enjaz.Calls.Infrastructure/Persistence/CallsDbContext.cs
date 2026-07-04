using Enjaz.Calls.Domain.Calls;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Calls.Infrastructure.Persistence;

public sealed class CallsDbContext(DbContextOptions<CallsDbContext> options) : DbContext(options)
{
    public DbSet<CallSession> CallSessions => Set<CallSession>();
    public DbSet<CallWebhookLog> CallWebhookLogs => Set<CallWebhookLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("calls");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CallsDbContext).Assembly);
    }
}
