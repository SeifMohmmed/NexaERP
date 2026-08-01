using FluentValidation;
using NexaERP.BLL.DTOs.PurchaseOrder;

namespace NexaERP.BLL.Validators.PurchaseOrder;

public sealed class UpdatePurchaseOrderStatusDtoValidator
    : AbstractValidator<UpdatePurchaseOrderStatusDto>
{
    public UpdatePurchaseOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
