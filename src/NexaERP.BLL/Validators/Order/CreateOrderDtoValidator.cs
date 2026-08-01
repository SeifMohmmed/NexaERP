using FluentValidation;
using NexaERP.BLL.DTOs.Order;
using NexaERP.BLL.DTOs.OrderLine;

namespace NexaERP.BLL.Validators.Order;

public sealed class CreateOrderDtoValidator
    : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator(
        IValidator<CreateOrderLineDto> lineValidator)
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Lines)
            .NotEmpty();

        RuleForEach(x => x.Lines)
            .SetValidator(lineValidator);
    }
}
