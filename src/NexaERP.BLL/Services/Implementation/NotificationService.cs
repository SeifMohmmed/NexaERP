using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Notifications;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Services;

namespace NexaERP.BLL.Services.Implementation;

public sealed class NotificationService(
    ApplicationDbContext dbContext,
    UserContext userContext,
    IValidator<NotificationPreferenceDto> preferenceValidator) : INotificationService
{
    public async Task<IReadOnlyList<NotificationDto>>
        GetMyNotificationsAsync(
            bool unreadOnly,
            int page,
            int pageSize,
            CancellationToken ct = default)
    {
        Guid userId = await GetCurrentUserIdAsync(ct);

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<Notification> query = dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => n.ReadAt == null);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto(
                n.Id,
                n.Type,
                n.Title,
                n.Body,
                n.ReadAt,
                n.CreatedAt,
                n.DeliveryStatus))
            .ToListAsync(ct);
    }

    public async Task<int> GetUnreadCountAsync(
        CancellationToken ct = default)
    {
        Guid userId = await GetCurrentUserIdAsync(ct);

        return await dbContext.Notifications
            .CountAsync(
                n => n.UserId == userId && n.ReadAt == null,
                ct);
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken ct = default)
    {
        Guid userId = await GetCurrentUserIdAsync(ct);

        Notification? notification =
            await dbContext.Notifications.FirstOrDefaultAsync(
                n => n.Id == notificationId && n.UserId == userId,
                ct);

        if (notification is null)
        {
            return false;
        }

        if (notification.ReadAt is null)
        {
            notification.ReadAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(ct);
        }

        return true;
    }

    public async Task<int> MarkAsReadAsync(
        IReadOnlyCollection<Guid> notificationIds,
        CancellationToken ct = default)
    {
        Guid userId = await GetCurrentUserIdAsync(ct);

        Guid[] ids = notificationIds
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
        {
            return 0;
        }

        DateTime now = DateTime.UtcNow;

        return await dbContext.Notifications
            .Where(n =>
                n.UserId == userId &&
                ids.Contains(n.Id) &&
                n.ReadAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    n => n.ReadAt,
                    now),
                ct);
    }

    public async Task<IReadOnlyList<NotificationPreferenceDto>>
        GetMyPreferencesAsync(
            CancellationToken ct = default)
    {
        Guid userId = await GetCurrentUserIdAsync(ct);

        return await dbContext.NotificationSettings
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Channel)
            .ThenBy(s => s.EventType)
            .Select(s => new NotificationPreferenceDto(
                s.Channel,
                s.EventType,
                s.Enabled))
            .ToListAsync(ct);
    }

    public async Task UpdateMyPreferencesAsync(
        UpdateNotificationPreferencesDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Preferences is null)
        {
            throw new ValidationException(
                "Preferences are required.");
        }

        NotificationPreferenceDto[] preferences =
            request.Preferences.ToArray();

        if (preferences.Length == 0)
        {
            throw new ValidationException(
                "At least one notification preference is required.");
        }

        if (preferences
            .GroupBy(p => new { p.Channel, p.EventType })
            .Any(g => g.Count() > 1))
        {
            throw new ValidationException(
                "Duplicate notification preferences are not allowed.");
        }

        foreach (NotificationPreferenceDto preference in preferences)
        {
            await preferenceValidator.ValidateAndThrowAsync(
                preference,
                ct);
        }

        Guid userId = await GetCurrentUserIdAsync(ct);

        foreach (NotificationPreferenceDto preference in preferences)
        {
            NotificationSettings? setting =
                await dbContext.NotificationSettings.FindAsync(
                    [
                        userId,
                        preference.Channel,
                        preference.EventType
                    ],
                    ct);

            if (setting is null)
            {
                setting = new NotificationSettings
                {
                    UserId = userId,
                    Channel = preference.Channel,
                    EventType = preference.EventType,
                    Enabled = preference.Enabled
                };

                dbContext.NotificationSettings.Add(setting);
            }
            else
            {
                setting.Enabled = preference.Enabled;
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }


    public async Task CreateAsync(
        Guid userId,
        string type,
        string title,
        string body,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        bool inAppEnabled = await dbContext.NotificationSettings
            .AsNoTracking()
            .Where(s =>
                s.UserId == userId &&
                s.Channel == "InApp" &&
                s.EventType == type)
            .Select(s => (bool?)s.Enabled)
            .FirstOrDefaultAsync(ct) ?? true;

        if (!inAppEnabled)
        {
            return;
        }

        Notification notification = new()
        {
            UserId = userId,
            Type = type,
            Title = title,
            Body = body,
            CreatedAt = DateTime.UtcNow,
            DeliveryStatus = "Delivered"
        };

        dbContext.Notifications.Add(notification);

        await dbContext.SaveChangesAsync(ct);
    }


    private async Task<Guid> GetCurrentUserIdAsync(
        CancellationToken ct)
    {
        Guid? userId = await userContext.GetUserIdAsync(ct);

        if (userId is null || userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "The current user could not be identified.");
        }

        return userId.Value;
    }
}
