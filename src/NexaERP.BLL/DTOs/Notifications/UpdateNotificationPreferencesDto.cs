namespace NexaERP.BLL.DTOs.Notifications;

public sealed record NotificationPreferenceDto(
    string Channel,
    string EventType,
    bool Enabled);

public sealed record UpdateNotificationPreferencesDto(
    IReadOnlyList<NotificationPreferenceDto> Preferences);
