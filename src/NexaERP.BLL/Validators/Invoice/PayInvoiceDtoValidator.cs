using FluentValidation;
using NexaERP.BLL.DTOs.Invoice;

namespace NexaERP.BLL.Validators.Invoice;

public sealed class PayInvoiceDtoValidator
    : AbstractValidator<PayInvoiceDto>
{
    public PayInvoiceDtoValidator()
    {
        RuleFor(x => x.PaidAt)
            .NotEmpty();

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .MaximumLength(50);
    }
}
