using NexaERP.BLL.DTOs.Automation;

namespace NexaERP.BLL.Services.Abstraction;

public interface IAutomationActionExecutor
{
    Task ExecuteAsync(
        AutomationActionDto action,
        Guid eventId,
        string eventType,
        string title,
        string body,
        CancellationToken ct = default);
}
