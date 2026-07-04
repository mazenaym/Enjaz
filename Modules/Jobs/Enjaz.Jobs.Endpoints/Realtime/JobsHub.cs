using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Enjaz.Jobs.Endpoints.Realtime;

[Authorize]
public sealed class JobsHub : Hub
{
    public async Task JoinJobGroup(Guid jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"job:{jobId}");
    }

    public async Task JoinMyGroups()
    {
        var userId = Context.User?.FindFirstValue("sub") ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userId, out var parsedUserId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"customer:{parsedUserId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"technician:{parsedUserId}");
        }

        if (Context.User?.IsInRole("Admin") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin:operations");
        }
    }
}
