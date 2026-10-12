namespace NexaERP.BLL.DTOs.Automation;

public sealed record UpdateAutomationRuleDto(
    string Name,
    string Trigger,
    AutomationActionDto Action);
