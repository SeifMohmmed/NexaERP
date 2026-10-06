using FluentValidation;
using NexaERP.BLL.DTOs.ProjectTask;

namespace NexaERP.BLL.Validators.ProjectTask;

public sealed class ChangeProjectTaskStatusDtoValidator
    : AbstractValidator<ChangeProjectTaskStatusDto>
{
    public ChangeProjectTaskStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
