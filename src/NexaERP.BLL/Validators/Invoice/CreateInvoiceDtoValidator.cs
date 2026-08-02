using FluentValidation;
using NexaERP.BLL.DTOs.InvoiceLine;

namespace NexaERP.BLL.Validators.Invoice;

public sealed class CreateInvoiceDtoValidator
    : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceDtoValidator(
        IValidator<CreateInvoiceLineDto> lineValidator)
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.InvoiceDate)
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.InvoiceDate);

        RuleFor(x => x.Lines)
            .NotEmpty();

        RuleForEach(x => x.Lines)
            .SetValidator(lineValidator);
    }
}
