using NexaERP.BLL.DTOs.Notifications;

namespace NexaERP.BLL.Services.Abstraction;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetMyNotificationsAsync(
        bool unreadOnly,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<int> GetUnreadCountAsync(
        CancellationToken ct = default);

    Task<bool> MarkAsReadAsync(
        Guid notificationId,
        CancellationToken ct = default);

    Task<int> MarkAsReadAsync(
        IReadOnlyCollection<Guid> notificationIds,
        CancellationToken ct = default);

    Task<IReadOnlyList<NotificationPreferenceDto>>
        GetMyPreferencesAsync(
            CancellationToken ct = default);

    Task UpdateMyPreferencesAsync(
        UpdateNotificationPreferencesDto request,
        CancellationToken ct = default);
    
    // Internal application use: create a notification for a specific user.
    Task CreateAsync(
        Guid userId,
        string type,
        string title,
        string body,
        CancellationToken ct = default);
}
