using FluentValidation;
using NexaERP.BLL.DTOs.ProjectTaskReport;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.Validators.TaskReport;

internal sealed class TaskReportQueryParametersValidator
    : AbstractValidator<TaskReportQueryParameters>
{
    public TaskReportQueryParametersValidator()
    {
        RuleFor(x => x.Status)
            .Must(status =>
                string.IsNullOrWhiteSpace(status) ||
                Enum.TryParse<ProjectTaskStatus>(
                    status,
                    true,
                    out _))
            .WithMessage(
                "Status must be ToDo, InProgress, Blocked, or Done.");

        RuleFor(x => x)
            .Must(x =>
                !x.From.HasValue ||
                !x.To.HasValue ||
                x.From <= x.To)
            .WithMessage(
                "From date must be before or equal to To date.");
    }
}
