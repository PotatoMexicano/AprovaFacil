using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AprovaFacil.Application.SignalR;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        String? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!String.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

            String? userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (!String.IsNullOrEmpty(userRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role-{userRole?.ToLower()}");
            }
        }

        await base.OnConnectedAsync();
    }
}
