using FluentValidation;
using NexaERP.BLL.DTOs.PurchaseOrder;

namespace NexaERP.BLL.Validators.PurchaseOrder;

public sealed class UpdatePurchaseOrderDtoValidator
    : AbstractValidator<UpdatePurchaseOrderDto>
{
    public UpdatePurchaseOrderDtoValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty();

        RuleFor(x => x.ExpectedDelivery)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);
    }
}
