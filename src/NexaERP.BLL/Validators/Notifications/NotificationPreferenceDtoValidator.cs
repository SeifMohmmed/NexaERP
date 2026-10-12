using FluentValidation;
using NexaERP.BLL.DTOs.Notifications;

namespace NexaERP.BLL.Validators.Notifications;

public sealed class NotificationPreferenceDtoValidator
    : AbstractValidator<NotificationPreferenceDto>
{
    private static readonly string[] SupportedChannels =
    [
        "InApp",
        "Email"
    ];

    private static readonly string[] SupportedEventTypes =
    [
        "InventoryBelowReorderLevel",
        "InvoiceOverdue"
    ];

    public NotificationPreferenceDtoValidator()
    {
        RuleFor(x => x.Channel)
            .NotEmpty()
            .Must(channel => SupportedChannels.Contains(
                channel,
                StringComparer.OrdinalIgnoreCase))
            .WithMessage("Channel must be InApp or Email.");

        RuleFor(x => x.EventType)
            .NotEmpty()
            .Must(eventType => SupportedEventTypes.Contains(
                eventType,
                StringComparer.OrdinalIgnoreCase))
            .WithMessage("Unsupported notification event type.");
    }
}
