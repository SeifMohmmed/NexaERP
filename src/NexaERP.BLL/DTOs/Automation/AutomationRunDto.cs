namespace NexaERP.BLL.DTOs.Automation;

public sealed record AutomationRunDto(
    Guid Id,
    Guid RuleId,
    string EventId,
    string Trigger,
    string Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    string? ErrorSummary);
