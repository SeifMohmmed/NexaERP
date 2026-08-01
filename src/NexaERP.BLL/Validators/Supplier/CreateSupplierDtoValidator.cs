using FluentValidation;
using NexaERP.BLL.DTOs.Supplier;

namespace NexaERP.BLL.Validators.Supplier;

public sealed class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierDtoValidator()
    {
        RuleFor(s => s.CompanyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(s => s.ContactName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(s => s.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(s => s.Phone)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(s => s.PaymentTerms)
            .NotEmpty()
            .MaximumLength(100);
    }
}
