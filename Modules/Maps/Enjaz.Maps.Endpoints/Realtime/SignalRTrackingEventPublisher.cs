using Enjaz.Maps.Application.Maps;
using Microsoft.AspNetCore.SignalR;

namespace Enjaz.Maps.Endpoints.Realtime;

public sealed class SignalRTrackingEventPublisher(IHubContext<TrackingHub> hubContext) : ITrackingEventPublisher
{
    public async Task PublishTechnicianLocationUpdatedAsync(TechnicianLocationUpdatedEvent updatedEvent, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients
            .Group("admin:operations")
            .SendAsync("technician.location.updated", updatedEvent, cancellationToken);
    }
}
