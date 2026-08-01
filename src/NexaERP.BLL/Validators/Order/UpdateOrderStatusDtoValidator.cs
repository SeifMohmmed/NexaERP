using FluentValidation;
using NexaERP.BLL.DTOs.Order;

namespace NexaERP.BLL.Validators.Order;

public sealed class UpdateOrderStatusDtoValidator
    : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
