using FluentValidation;
using NexaERP.BLL.DTOs.OrderLine;

namespace NexaERP.BLL.Validators.OrderLine;

public sealed class CreateOrderLineDtoValidator
    : AbstractValidator<CreateOrderLineDto>
{
    public CreateOrderLineDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0);
    }
}
