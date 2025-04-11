using AprovaFacil.Application.SignalR;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AprovaFacil.Application.Services;

public class NotificationService(IHubContext<NotificationHub> hubNotification) : NotificationInterfaces.INotificationService
{
    public async Task NotifyBroadcast(CancellationToken cancellation)
    {
        await hubNotification.Clients.All.SendAsync("UpdateRequests", new { }, cancellation);
    }

    public async Task NotifyRegisterAsync(NotificationRequestDTO notificationRequest, CancellationToken cancellation)
    {
        await hubNotification.Clients.Group($"user-{notificationRequest.RequesterID}")
            .SendAsync("UpdateRequests", new
            {
                notificationRequest.RequestUUID
            }, cancellation);

        foreach (Int32 manager in notificationRequest.ManagerIds)
        {
            await hubNotification.Clients.Group($"user-{manager}")
                .SendAsync("UpdateRequests", new
                {
                    notificationRequest.RequestUUID
                }, cancellation);
        }
    }

}
