using FluentValidation;
using NexaERP.BLL.DTOs.Project;

namespace NexaERP.BLL.Validators.Project;

public sealed class CreateProjectDtoValidator
    : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
