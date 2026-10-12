namespace NexaERP.BLL.DTOs.Automation;

public sealed record AutomationActionDto(
    string Type,
    string? Role = null,
    Guid? UserId = null);
