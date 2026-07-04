using Enjaz.Jobs.Application.Jobs;
using Microsoft.AspNetCore.SignalR;

namespace Enjaz.Jobs.Endpoints.Realtime;

public sealed class SignalRJobsEventPublisher(IHubContext<JobsHub> hubContext) : IJobsEventPublisher
{
    public async Task PublishAsync(JobEventMessage message, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.Group($"job:{message.JobId}").SendAsync(message.EventName, message, cancellationToken);

        if (message.CustomerUserId.HasValue)
        {
            await hubContext.Clients.Group($"customer:{message.CustomerUserId.Value}").SendAsync(message.EventName, message, cancellationToken);
        }

        if (message.TechnicianUserId.HasValue)
        {
            await hubContext.Clients.Group($"technician:{message.TechnicianUserId.Value}").SendAsync(message.EventName, message, cancellationToken);
        }

        await hubContext.Clients.Group("admin:operations").SendAsync(message.EventName, message, cancellationToken);
    }
}
