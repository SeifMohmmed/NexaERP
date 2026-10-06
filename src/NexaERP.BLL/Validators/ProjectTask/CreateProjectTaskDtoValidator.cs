using FluentValidation;
using NexaERP.BLL.DTOs.ProjectTask;

namespace NexaERP.BLL.Validators.ProjectTask;

public sealed class CreateProjectTaskDtoValidator
    : AbstractValidator<CreateProjectTaskDto>
{
    public CreateProjectTaskDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.AssigneeId)
            .NotEmpty();

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.DueDate)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
