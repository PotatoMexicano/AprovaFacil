using AprovaFacil.Domain.DTOs;

namespace AprovaFacil.Domain.Interfaces;

public static class NotificationInterfaces
{
    public interface INotificationService
    {
        Task NotifyRegisterAsync(NotificationRequestDTO notificationRequest, CancellationToken cancellation);
        Task NotifyBroadcast(CancellationToken cancellation);
    }
}
