using NexaERP.BLL.DTOs.Automation;
using NexaERP.BLL.Services.Abstraction;

namespace NexaERP.BLL.Services.Implementation;

public sealed class AutomationActionExecutor(
    INotificationService notificationService,
    IIdentityRecipientService recipientService)
    : IAutomationActionExecutor
{
    public async Task ExecuteAsync(
        AutomationActionDto action,
        Guid eventId,
        string eventType,
        string title,
        string body,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(action.Type);
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        switch (action.Type)
        {
            case "NotifyRole":
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(action.Role);

                IReadOnlyList<Guid> userIds =
                    await recipientService.GetUserIdsByRoleAsync(
                        action.Role,
                        ct);

                foreach (Guid userId in userIds)
                {
                    await notificationService.CreateAsync(
                        userId,
                        eventType,
                        title,
                        body,
                        ct);
                }

                break;
            }

            case "NotifyUser":
            {
                if (action.UserId is null ||
                    action.UserId == Guid.Empty)
                {
                    throw new ArgumentException(
                        "UserId is required for NotifyUser.",
                        nameof(action));
                }

                bool userExists =
                    await recipientService.UserExistsAsync(
                        action.UserId.Value,
                        ct);

                if (!userExists)
                {
                    throw new InvalidOperationException(
                        $"User '{action.UserId}' was not found.");
                }

                await notificationService.CreateAsync(
                    action.UserId.Value,
                    eventType,
                    title,
                    body,
                    ct);

                break;
            }

            default:
                throw new NotSupportedException(
                    $"Action type '{action.Type}' is not supported.");
        }
    }
}
