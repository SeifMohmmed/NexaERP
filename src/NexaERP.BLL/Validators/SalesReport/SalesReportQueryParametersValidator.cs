using FluentValidation;
using NexaERP.BLL.DTOs.SalesReport;

namespace NexaERP.BLL.Validators.SalesReport;

internal sealed class SalesReportQueryParametersValidator
    : AbstractValidator<SalesReportQueryParameters>
{
    public SalesReportQueryParametersValidator()
    {
        RuleFor(x => x.GroupBy)
            .Must(value =>
                value.Equals("day", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("week", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("month", StringComparison.OrdinalIgnoreCase))
            .WithMessage("GroupBy must be day, week, or month.");

        RuleFor(x => x)
            .Must(x =>
                !x.From.HasValue ||
                !x.To.HasValue ||
                x.From <= x.To)
            .WithMessage("From date must be before or equal to To date.");
    }
}
