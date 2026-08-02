using FluentValidation;
using NexaERP.BLL.DTOs.Invoice;

namespace NexaERP.BLL.Validators.Invoice;

public sealed class UpdateInvoiceDtoValidator
    : AbstractValidator<UpdateInvoiceDto>
{
    public UpdateInvoiceDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.InvoiceDate)
            .NotEmpty();

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.InvoiceDate);
    }
}
