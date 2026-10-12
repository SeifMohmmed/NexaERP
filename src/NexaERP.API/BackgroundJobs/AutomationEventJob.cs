using NexaERP.BLL.Services.Abstraction;

namespace NexaERP.API.BackgroundJobs;

public sealed class AutomationEventJob(
    IAutomationEventProcessor automationEventProcessor)
{
    public async Task ExecuteAsync(
        Guid eventId,
        string trigger,
        string title,
        string body)
    {
        await automationEventProcessor.ProcessAsync(
            eventId,
            trigger,
            title,
            body,
            CancellationToken.None);
    }
}
