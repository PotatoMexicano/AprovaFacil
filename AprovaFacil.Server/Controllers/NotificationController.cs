using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Identity;
using AprovaFacil.Server.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/notification")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationInterfaces.INotificationRepository _notificationRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationController(UserManager<ApplicationUser> userManager, NotificationInterfaces.INotificationRepository notificationRepository)
    {
        _userManager = userManager;
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Notifications(CancellationToken cancellation = default)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "User unauthorized",
            });
        }

        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "User not found"
            });
        }

        Int32? userId = User.FindUserIdentifier();

        if (!userId.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Detail = "User not found"
            });
        }

        IEnumerable<Notification> notifications = await _notificationRepository.GetNotificationsAsync(userId.Value, cancellation);

        return Ok(notifications);
    }

    [HttpPost("{guidNotification}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(String guidNotification, CancellationToken cancellation = default)
    {
        if (!Guid.TryParse(guidNotification, out Guid notificationUUID))
        {
            return BadRequest(new ProblemDetails
            {
                Detail = "Notification NameIdentifier not valid."
            });
        }

        Notification? notification = await _notificationRepository.MarkAsReadAsync(notificationUUID, cancellation);

        if (notification is null)
        {
            return NotFound(new ProblemDetails
            {
                Detail = "Notification not found."
            });
        }

        return NoContent();

    }
}
