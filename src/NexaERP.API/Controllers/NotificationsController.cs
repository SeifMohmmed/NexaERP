using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Notifications;
using NexaERP.BLL.Services.Abstraction;

namespace NexaERP.API.Controllers;

[ApiController]
[Route("notifications")]
[Authorize]
public sealed class NotificationsController(
    INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>>
        GetMyNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
    {
        var notifications =
            await notificationService.GetMyNotificationsAsync(
                unreadOnly,
                page,
                pageSize,
                ct);

        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(
        CancellationToken ct = default)
    {
        int count =
            await notificationService.GetUnreadCountAsync(ct);

        return Ok(new { unreadCount = count });
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken ct = default)
    {
        bool updated =
            await notificationService.MarkAsReadAsync(id, ct);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("read")]
    public async Task<IActionResult> MarkSelectedAsRead(
        [FromBody] MarkNotificationsReadDto request,
        CancellationToken ct = default)
    {
        if (request.NotificationIds is null ||
            request.NotificationIds.Count == 0)
        {
            return BadRequest(new
            {
                message = "At least one notification ID is required."
            });
        }

        int updated =
            await notificationService.MarkAsReadAsync(
                request.NotificationIds,
                ct);

        return Ok(new { updatedCount = updated });
    }

    [HttpGet("~/notification-preferences")]
    public async Task<
        ActionResult<IReadOnlyList<NotificationPreferenceDto>>>
        GetMyPreferences(CancellationToken ct = default)
    {
        var preferences =
            await notificationService.GetMyPreferencesAsync(ct);

        return Ok(preferences);
    }

    [HttpPut("~/notification-preferences")]
    public async Task<IActionResult> UpdateMyPreferences(
        [FromBody] UpdateNotificationPreferencesDto request,
        CancellationToken ct = default)
    {
        await notificationService.UpdateMyPreferencesAsync(
            request,
            ct);

        return NoContent();
    }
}
