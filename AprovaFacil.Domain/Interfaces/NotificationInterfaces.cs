using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class NotificationInterfaces
{
    // This is a notification for update data in RealTime using SignalR.
    public interface INotificationService
    {
        Task NotifyBroadcast(CancellationToken cancellation);
        Task NotifyUsers(NotificationRequest notificationRequest, CancellationToken cancellation);
        Task NotifyGroup(NotificationGroupRequest notificationRequest, CancellationToken cancellation);
    }

    // This is a notificiation for alert user.
    public interface INotificationRepository
    {
        Task SaveNotifyAsync(NotificationRequest notificationRequest);

        Task<IEnumerable<Notification>> GetNotificationsAsync(Int32 userId, CancellationToken cancellation);
        Task<Notification?> MarkAsReadAsync(Guid notificationUUID, CancellationToken cancellation);
    }
}
