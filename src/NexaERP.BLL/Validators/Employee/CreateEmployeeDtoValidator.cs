using FluentValidation;
using NexaERP.BLL.DTOs.Employee;

namespace NexaERP.BLL.Validators.Employee;

public sealed class CreateEmployeeDtoValidator
    : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.Position)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.HireDate)
            .LessThanOrEqualTo(DateTime.UtcNow);

        RuleFor(x => x.Salary)
            .GreaterThan(0);
    }
}
