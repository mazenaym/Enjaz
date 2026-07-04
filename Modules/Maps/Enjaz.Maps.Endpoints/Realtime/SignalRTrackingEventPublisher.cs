using Enjaz.Jobs.Endpoints.Realtime;
using Enjaz.Maps.Application.Maps;
using Microsoft.AspNetCore.SignalR;

namespace Enjaz.Maps.Endpoints.Realtime;

public sealed class SignalRTrackingEventPublisher(IHubContext<TrackingHub> hubContext, IHubContext<JobsHub> jobsHubContext) : ITrackingEventPublisher
{
    public async Task PublishTechnicianLocationUpdatedAsync(TechnicianLocationUpdatedEvent updatedEvent, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Group("admin:operations")
            .SendAsync("technician.location.updated", updatedEvent, cancellationToken);

        if (updatedEvent.JobId.HasValue)
        {
            await jobsHubContext.Clients.Group($"job:{updatedEvent.JobId.Value}").SendAsync("technician.job.location.updated", updatedEvent, cancellationToken);
            await jobsHubContext.Clients.Group("admin:operations").SendAsync("technician.job.location.updated", updatedEvent, cancellationToken);
        }

        if (updatedEvent.CustomerUserId.HasValue)
        {
            await jobsHubContext.Clients.Group($"customer:{updatedEvent.CustomerUserId.Value}").SendAsync("technician.job.location.updated", updatedEvent, cancellationToken);
        }

        if (updatedEvent.TechnicianUserId.HasValue)
        {
            await jobsHubContext.Clients.Group($"technician:{updatedEvent.TechnicianUserId.Value}").SendAsync("technician.job.location.updated", updatedEvent, cancellationToken);
        }
    }
}
