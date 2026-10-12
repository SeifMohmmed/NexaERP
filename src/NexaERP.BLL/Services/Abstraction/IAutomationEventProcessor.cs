namespace NexaERP.BLL.Services.Abstraction;

public interface IAutomationEventProcessor
{
    Task ProcessAsync(
        Guid eventId,
        string trigger,
        string title,
        string body,
        CancellationToken ct = default);
}
