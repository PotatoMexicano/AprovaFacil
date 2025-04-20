using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class NotificationInterfaces
{
    public interface INotificationService
    {
        Task NotifyBroadcast(CancellationToken cancellation);
        Task NotifyUsers(NotificationRequest notificationRequest, CancellationToken cancellation);

        Task NotifyGroup(NotificationGroupRequest notificationRequest, CancellationToken cancellation);
    }

    public interface INotificationRepository
    {
        Task SaveNotifyAsync(NotificationRequest notificationRequest, CancellationToken cancellation);
        Task<IEnumerable<Notification>> GetNotificationsAsync(Int32 userId, CancellationToken cancellation);
        Task<Notification?> MarkAsReadAsync(Guid notificationUUID, CancellationToken cancellation);
    }
}
