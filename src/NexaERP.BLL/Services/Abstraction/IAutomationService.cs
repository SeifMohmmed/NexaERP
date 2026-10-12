using NexaERP.BLL.DTOs.Automation;

namespace NexaERP.BLL.Services.Abstraction;

public interface IAutomationService
{
    Task<IReadOnlyList<AutomationRuleDto>> GetRulesAsync(
        CancellationToken ct = default);

    Task<AutomationRuleDto?> GetRuleByIdAsync(
        Guid id,
        CancellationToken ct = default);

    Task<AutomationRuleDto> CreateRuleAsync(
        CreateAutomationRuleDto request,
        Guid createdBy,
        CancellationToken ct = default);

    Task<AutomationRuleDto?> UpdateRuleAsync(
        Guid id,
        UpdateAutomationRuleDto request,
        CancellationToken ct = default);

    Task<bool> DeleteRuleAsync(
        Guid id,
        CancellationToken ct = default);

    Task<bool> SetRuleEnabledAsync(
        Guid id,
        bool enabled,
        CancellationToken ct = default);
    
        Task<IReadOnlyList<AutomationRunDto>> GetRunsAsync(
        Guid? ruleId,
        string? status,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default);

    Task<AutomationRunDto?> GetRunByIdAsync(
            Guid id,
            CancellationToken ct = default);
}
