using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AprovaFacil.Server.Controllers;

[Route("api/notification")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationInterfaces.INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationController(UserManager<ApplicationUser> userManager, NotificationInterfaces.INotificationService notificationService)
    {
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Notifications(CancellationToken cancellation = default)
    {
        //if (User.Identity == null || !User.Identity.IsAuthenticated)
        //{
        //    return Unauthorized(new ProblemDetails
        //    {
        //        Status = StatusCodes.Status401Unauthorized,
        //        Title = "User unauthorized",
        //    });
        //}

        //ApplicationUser? user = await _userManager.GetUserAsync(User);
        //if (user == null)
        //{
        //    return Unauthorized(new ProblemDetails
        //    {
        //        Status = StatusCodes.Status404NotFound,
        //        Detail = "User not found"
        //    });
        //}

        //if (!Int32.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Int32 userId))
        //{
        //    return Unauthorized(new ProblemDetails
        //    {
        //        Status = StatusCodes.Status404NotFound,
        //        Detail = "User not found"
        //    });
        //}

        //IEnumerable<Notification> notifications = await _notificationService.GetNotifications(userId, cancellation);

        //return Ok(notifications);
        return Ok();
    }

    [HttpPost("{guidNotification}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(String guidNotification, CancellationToken cancellation = default)
    {
        //if (!Guid.TryParse(guidNotification, out Guid notificationUUID))
        //{
        //    return BadRequest(new ProblemDetails
        //    {
        //        Detail = "Notification NameIdentifier not valid."
        //    });
        //}

        //Notification? notification = await _notificationService.MarkAsRead(notificationUUID, cancellation);

        //if (notification is null)
        //{
        //    return NotFound(new ProblemDetails
        //    {
        //        Detail = "Notification not found."
        //    });
        //}

        return NoContent();

    }
}
