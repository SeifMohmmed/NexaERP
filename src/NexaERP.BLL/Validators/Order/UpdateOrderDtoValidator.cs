using FluentValidation;
using NexaERP.BLL.DTOs.Order;

namespace NexaERP.BLL.Validators.Order;

public sealed class UpdateOrderDtoValidator
    : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ShippingAddress)
            .NotEmpty()
            .MaximumLength(250);
    }
}
