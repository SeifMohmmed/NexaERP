using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.Mappings;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;
using Npgsql;

namespace NexaERP.BLL.Services.Implementation;

public sealed class AutomationEventProcessor(
    IGenericRepository<AutomationRule> ruleRepository,
    IAutomationActionExecutor actionExecutor,
    ApplicationDbContext dbContext)
    : IAutomationEventProcessor
{
    public async Task ProcessAsync(
        Guid eventId,
        string trigger,
        string title,
        string body,
        CancellationToken ct = default)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event ID cannot be empty.",
                nameof(eventId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(trigger);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        var rules = await ruleRepository.GetAllAsync(ct: ct);

        var matchingRules = rules
            .Where(rule =>
                rule.Enabled &&
                string.Equals(
                    rule.Trigger,
                    trigger,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        var eventKey = eventId.ToString();

        foreach (AutomationRule rule in matchingRules)
        {
            ct.ThrowIfCancellationRequested();

            var alreadyProcessed = await dbContext.AutomationRuns
                .AnyAsync(
                    run => run.RuleId == rule.Id &&
                           run.EventId == eventKey,
                    ct);

            if (alreadyProcessed)
            {
                continue;
            }

            var run = new AutomationRun
            {
                Id = Guid.NewGuid(),
                RuleId = rule.Id,
                EventId = eventKey,
                Trigger = trigger,
                Status = "Processing",
                StartedAt = DateTime.UtcNow
            };

            dbContext.AutomationRuns.Add(run);

            try
            {
                // Save the run before executing the action.
                // The unique index protects against concurrent duplicates.
                await dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
                when (IsDuplicateAutomationRun(ex))
            {
                // Another processor claimed this rule and event.
                dbContext.Entry(run).State = EntityState.Detached;
                continue;
            }

            try
            {
                var action = AutomationMapping.ToDto(rule).Action;

                await actionExecutor.ExecuteAsync(
                    action,
                    eventId,
                    trigger,
                    title,
                    body,
                    ct);

                run.Status = "Completed";
                run.CompletedAt = DateTime.UtcNow;
                run.ErrorSummary = null;

                await dbContext.SaveChangesAsync(ct);
            }
            catch (OperationCanceledException)
                when (ct.IsCancellationRequested)
            {
                run.Status = "Failed";
                run.CompletedAt = DateTime.UtcNow;
                run.ErrorSummary = "Execution was canceled.";

                await dbContext.SaveChangesAsync(
                    CancellationToken.None);

                throw;
            }
            catch (Exception)
            {
                run.Status = "Failed";
                run.CompletedAt = DateTime.UtcNow;
                run.ErrorSummary =
                    "Automation action execution failed.";

                await dbContext.SaveChangesAsync(
                    CancellationToken.None);

                // Continue processing the remaining matching rules.
            }
        }
    }

    private static bool IsDuplicateAutomationRun(
        DbUpdateException exception)
    {
        // Check if the exception is due to a unique constraint violation in PostgreSQL.
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: "ix_automation_runs_rule_id_event_id"
        };
    }
}
