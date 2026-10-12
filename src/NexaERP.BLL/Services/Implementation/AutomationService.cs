using NexaERP.BLL.DTOs.Automation;
using NexaERP.BLL.Mappings;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.BLL.Services.Implementation;

public sealed class AutomationService(
    IGenericRepository<AutomationRule> ruleRepository,
    IGenericRepository<AutomationRun> runRepository,
    IUnitOfWork unitOfWork) : IAutomationService
{
    // =========================
    // Automation Rules
    // =========================

    public async Task<IReadOnlyList<AutomationRuleDto>> GetRulesAsync(
        CancellationToken ct = default)
    {
        var rules = await ruleRepository.GetAllAsync(ct: ct);

        return rules
            .Select(rule => rule.ToDto())
            .ToList();
    }

    public async Task<AutomationRuleDto?> GetRuleByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var rule = await ruleRepository.GetByIdAsync(id, ct);

        return rule?.ToDto();
    }

    public async Task<AutomationRuleDto> CreateRuleAsync(
        CreateAutomationRuleDto request,
        Guid createdBy,
        CancellationToken ct = default)
    {
        var rule = new AutomationRule
        {
            Name = request.Name,
            Trigger = request.Trigger,
            Action = AutomationMapping.SerializeAction(request.Action),
            Enabled = request.Enabled,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await ruleRepository.AddAsync(rule);
        await unitOfWork.SaveChangesAsync(ct);

        return rule.ToDto();
    }

    public async Task<AutomationRuleDto?> UpdateRuleAsync(
        Guid id,
        UpdateAutomationRuleDto request,
        CancellationToken ct = default)
    {
        var rule = await ruleRepository.GetByIdAsync(id, ct);

        if (rule is null)
        {
            return null;
        }

        rule.Name = request.Name;
        rule.Trigger = request.Trigger;
        rule.Action = AutomationMapping.SerializeAction(request.Action);
        rule.UpdatedAt = DateTime.UtcNow;

        ruleRepository.Update(rule);
        await unitOfWork.SaveChangesAsync(ct);

        return rule.ToDto();
    }

    public async Task<bool> DeleteRuleAsync(
        Guid id,
        CancellationToken ct = default)
    {
        AutomationRule? rule = await ruleRepository.GetByIdAsync(id, ct);

        if (rule is null)
        {
            return false;
        }

        ruleRepository.Delete(rule);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> SetRuleEnabledAsync(
        Guid id,
        bool enabled,
        CancellationToken ct = default)
    {
        AutomationRule? rule = await ruleRepository.GetByIdAsync(id, ct);

        if (rule is null)
        {
            return false;
        }

        rule.Enabled = enabled;
        rule.UpdatedAt = DateTime.UtcNow;

        ruleRepository.Update(rule);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    // =========================
    // Automation Runs
    // =========================

    public async Task<IReadOnlyList<AutomationRunDto>> GetRunsAsync(
        Guid? ruleId,
        string? status,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default)
    {
        var runs = await runRepository.GetAllAsync(
            run =>
                (!ruleId.HasValue || run.RuleId == ruleId.Value) &&
                (string.IsNullOrWhiteSpace(status) ||
                 run.Status == status) &&
                (!from.HasValue || run.StartedAt >= from.Value) &&
                (!to.HasValue || run.StartedAt <= to.Value),
            ct);

        return runs
            .OrderByDescending(run => run.StartedAt)
            .Select(run => new AutomationRunDto(
                run.Id,
                run.RuleId,
                run.EventId,
                run.Trigger,
                run.Status,
                run.StartedAt,
                run.CompletedAt,
                run.ErrorSummary))
            .ToList();
    }

    public async Task<AutomationRunDto?> GetRunByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        var run = await runRepository.GetByIdAsync(id, ct);

        if (run is null)
        {
            return null;
        }

        return new AutomationRunDto(
            run.Id,
            run.RuleId,
            run.EventId,
            run.Trigger,
            run.Status,
            run.StartedAt,
            run.CompletedAt,
            run.ErrorSummary);
    }
}
