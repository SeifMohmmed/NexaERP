using System.Text.Json;
using NexaERP.BLL.DTOs.Automation;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class AutomationMapping
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static AutomationRuleDto ToDto(this AutomationRule rule)
    {
        AutomationActionDto action =
            JsonSerializer.Deserialize<AutomationActionDto>(
                rule.Action,
                JsonOptions)
            ?? throw new InvalidOperationException(
                $"Automation rule '{rule.Id}' has an invalid action configuration.");

        return new AutomationRuleDto(
            rule.Id,
            rule.Name,
            rule.Trigger,
            action,
            rule.Enabled,
            rule.CreatedBy,
            rule.CreatedAt,
            rule.UpdatedAt);
    }

    public static AutomationRunDto ToDto(this AutomationRun run)
    {
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

    public static string SerializeAction(AutomationActionDto action)
    {
        return JsonSerializer.Serialize(action, JsonOptions);
    }
}
