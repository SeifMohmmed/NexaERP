using FluentValidation;
using NexaERP.BLL.DTOs.Department;

namespace NexaERP.BLL.Validators.Department;

public sealed class CreateDepartmentDtoValidator
    : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
