using FluentValidation;
using NexaERP.BLL.DTOs.InvoiceLine;

namespace NexaERP.BLL.Validators.InvoiceLine;

public sealed class CreateInvoiceLineDtoValidator
    : AbstractValidator<CreateInvoiceLineDto>
{
    public CreateInvoiceLineDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);

        RuleFor(x => x.TaxRate)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100);
    }
}
