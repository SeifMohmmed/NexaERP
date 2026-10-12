namespace NexaERP.DAL.Entities;

public sealed class AutomationRun : Entity
{
    public Guid RuleId { get; set; }

    // Identifier of the business event being processed.
    public string EventId { get; set; } = string.Empty;

    public string Trigger { get; set; } = string.Empty;

    // Queued, Processing, Completed, or Failed.
    public string Status { get; set; } = "Queued";

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Safe error summary; never store secrets here.
    public string? ErrorSummary { get; set; }

    public AutomationRule Rule { get; set; } = null!;
}
