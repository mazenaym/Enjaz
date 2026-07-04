using Enjaz.Support.Domain.Support;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Support.Infrastructure.Persistence;

public sealed class SupportDbContext(DbContextOptions<SupportDbContext> options) : DbContext(options)
{
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SupportTicketMessage> SupportTicketMessages => Set<SupportTicketMessage>();
    public DbSet<JobDispute> JobDisputes => Set<JobDispute>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("support");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportDbContext).Assembly);
    }
}
