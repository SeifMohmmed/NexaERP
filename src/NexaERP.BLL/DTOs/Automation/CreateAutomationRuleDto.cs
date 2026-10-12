namespace NexaERP.BLL.DTOs.Automation;

public sealed record CreateAutomationRuleDto(
    string Name,
    string Trigger,
    AutomationActionDto Action,
    bool Enabled = true);
