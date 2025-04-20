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

    public async Task NotifyGroup(NotificationGroupRequest request, CancellationToken cancellation)
    {
        IEnumerable<Task> tasks = request.Groups.Select(group => hubNotification.Clients.Group(group).SendAsync("UpdateApproved", new { request.RequestUUID }, cancellation));
        await Task.WhenAll(tasks);
    }

    public async Task NotifyUsers(NotificationRequest request, CancellationToken cancellation)
    {
        IEnumerable<Task> tasks = request.UsersID.Select(userId => hubNotification.Clients.Group($"user-{userId}").SendAsync("UpdateRequests", new { request.RequestUUID }, cancellation));
        await Task.WhenAll(tasks);
    }
}
