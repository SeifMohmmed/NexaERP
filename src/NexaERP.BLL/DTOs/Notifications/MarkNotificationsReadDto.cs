namespace NexaERP.BLL.DTOs.Notifications;

public sealed record MarkNotificationsReadDto(
    IReadOnlyList<Guid> NotificationIds);
