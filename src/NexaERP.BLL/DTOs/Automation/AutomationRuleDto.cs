namespace NexaERP.BLL.DTOs.Automation;

public sealed record AutomationRuleDto(
    Guid Id,
    string Name,
    string Trigger,
    AutomationActionDto Action,
    bool Enabled,
    Guid CreatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
