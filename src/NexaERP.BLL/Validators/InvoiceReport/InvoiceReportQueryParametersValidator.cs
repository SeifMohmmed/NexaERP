using FluentValidation;
using NexaERP.BLL.DTOs.InvoiceReport;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.Validators.InvoiceReport;

public sealed class InvoiceReportQueryParametersValidator
    : AbstractValidator<InvoiceReportQueryParameters>
{
    public InvoiceReportQueryParametersValidator()
    {
        RuleFor(x => x.Status)
            .Must(status =>
                string.IsNullOrWhiteSpace(status) ||
                Enum.TryParse<InvoiceStatus>(
                    status,
                    true,
                    out _))
            .WithMessage(
                "Status must be Draft, Issued, or Paid.");

        RuleFor(x => x)
            .Must(x =>
                !x.From.HasValue ||
                !x.To.HasValue ||
                x.From <= x.To)
            .WithMessage(
                "From date must be before or equal to To date.");
    }
}
