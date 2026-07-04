using Enjaz.Calls.Application.Calls;
using Enjaz.Calls.Domain.Calls;
using Enjaz.Calls.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Enjaz.Calls.Infrastructure.Calls;

public sealed class CallsRepository(CallsDbContext dbContext) : ICallsRepository
{
    public IQueryable<CallSession> QuerySessions() => dbContext.CallSessions;
    public IQueryable<CallWebhookLog> QueryWebhookLogs() => dbContext.CallWebhookLogs;
    public Task<CallSession?> GetSessionAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.CallSessions.FirstOrDefaultAsync(call => call.Id == id, cancellationToken);
    public Task<CallSession?> GetSessionByProviderCallIdAsync(string providerCallId, CancellationToken cancellationToken = default) => dbContext.CallSessions.FirstOrDefaultAsync(call => call.ProviderCallId == providerCallId, cancellationToken);
    public Task<CallWebhookLog?> GetWebhookLogAsync(Guid id, CancellationToken cancellationToken = default) => dbContext.CallWebhookLogs.FirstOrDefaultAsync(log => log.Id == id, cancellationToken);
    public async Task AddSessionAsync(CallSession session, CancellationToken cancellationToken = default) => await dbContext.CallSessions.AddAsync(session, cancellationToken);
    public async Task AddWebhookLogAsync(CallWebhookLog log, CancellationToken cancellationToken = default) => await dbContext.CallWebhookLogs.AddAsync(log, cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
