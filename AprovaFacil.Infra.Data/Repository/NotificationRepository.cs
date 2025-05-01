using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Repository;

public class NotificationRepository(ApplicationDbContext context) : NotificationInterfaces.INotificationRepository
{
    public async Task<IEnumerable<Notification>> GetNotificationsAsync(Int32 userId, CancellationToken cancellation)
    {

        Notification[] notifications = await context.Notifications
            .Where(n => n.UserId == userId && n.ExpireAt >= DateTime.UtcNow && n.Opened == false)
            .Select(x => new Notification
            {
                CreateAt = x.CreateAt,
                ExpireAt = x.ExpireAt,
                Message = x.Message,
                UserId = x.UserId,
                UUID = x.UUID,
                RequestUUID = x.RequestUUID,
            })
            .AsNoTracking()
            .ToArrayAsync(cancellation);
        return notifications;
    }

    public async Task<Notification?> MarkAsReadAsync(Guid notificationUUID, CancellationToken cancellation)
    {
        Notification? notification = await context.Notifications.Where(x => x.UUID == notificationUUID).FirstOrDefaultAsync(cancellation);

        if (notification is null)
        {
            return null;
        }

        notification.Opened = true;

        await context.SaveChangesAsync(cancellation);

        return notification;
    }

    public async Task SaveNotifyAsync(NotificationRequest notificationRequest)
    {
        Notification[] notifications = notificationRequest.UsersID.Select(req =>
        {
            return new Notification
            {
                Message = notificationRequest.Message,
                CreateAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddDays(10),
                Opened = false,
                RequestUUID = notificationRequest.RequestUUID,
                UserId = req,
            };
        }).ToArray();

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
    }
}
