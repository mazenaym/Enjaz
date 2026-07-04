using Enjaz.Support.Application.Support;
using Enjaz.Support.Domain.Support;
using Enjaz.Support.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Support.Infrastructure.Support;

public sealed class SupportRepository(SupportDbContext dbContext) : ISupportRepository
{
    public IQueryable<SupportTicket> QueryTickets() => dbContext.SupportTickets;
    public IQueryable<SupportTicketMessage> QueryMessages() => dbContext.SupportTicketMessages;
    public IQueryable<JobDispute> QueryDisputes() => dbContext.JobDisputes;
    public Task<SupportTicket?> GetTicketAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.SupportTickets.FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
    public Task<JobDispute?> GetDisputeAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.JobDisputes.FirstOrDefaultAsync(dispute => dispute.Id == id, cancellationToken);
    public async Task AddTicketAsync(SupportTicket ticket, CancellationToken cancellationToken = default) => await dbContext.SupportTickets.AddAsync(ticket, cancellationToken);
    public async Task AddMessageAsync(SupportTicketMessage message, CancellationToken cancellationToken = default) => await dbContext.SupportTicketMessages.AddAsync(message, cancellationToken);
    public async Task AddDisputeAsync(JobDispute dispute, CancellationToken cancellationToken = default) => await dbContext.JobDisputes.AddAsync(dispute, cancellationToken);

    public async Task<string> GenerateTicketNumberAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var prefix = $"SUP-{nowUtc:yyyyMMdd}";
        var count = await dbContext.SupportTickets.CountAsync(ticket => ticket.TicketNumber.StartsWith(prefix), cancellationToken);
        return $"{prefix}-{count + 1:0000}";
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
