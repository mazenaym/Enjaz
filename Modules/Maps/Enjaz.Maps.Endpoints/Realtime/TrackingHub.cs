using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Enjaz.Maps.Endpoints.Realtime;

[Authorize]
public sealed class TrackingHub : Hub
{
    public async Task JoinOperationsGroup()
    {
        if (Context.User?.IsInRole("Admin") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin:operations");
        }
    }
}
