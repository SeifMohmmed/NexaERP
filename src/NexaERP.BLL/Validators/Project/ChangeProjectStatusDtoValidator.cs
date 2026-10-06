using FluentValidation;
using NexaERP.BLL.DTOs.Project;

namespace NexaERP.BLL.Validators.Project;

public sealed class ChangeProjectStatusDtoValidator
    : AbstractValidator<ChangeProjectStatusDto>
{
    public ChangeProjectStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
