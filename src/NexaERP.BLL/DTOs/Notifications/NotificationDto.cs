namespace NexaERP.BLL.DTOs.Notifications;

public sealed record NotificationDto(
    Guid Id,
    string Type,
    string Title,
    string Body,
    DateTime? ReadAt,
    DateTime CreatedAt,
    string? DeliveryStatus);
