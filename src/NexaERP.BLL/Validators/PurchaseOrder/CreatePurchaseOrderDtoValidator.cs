using FluentValidation;
using NexaERP.BLL.DTOs.PurchaseLine;
using NexaERP.BLL.DTOs.PurchaseOrder;

namespace NexaERP.BLL.Validators.PurchaseOrder;

public sealed class CreatePurchaseOrderDtoValidator
    : AbstractValidator<CreatePurchaseOrderDto>
{
    public CreatePurchaseOrderDtoValidator(
    IValidator<CreatePurchaseLineDto> lineValidator)
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty();

        RuleFor(x => x.ExpectedDelivery)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.Lines)
            .NotEmpty();

        RuleForEach(x => x.Lines)
            .SetValidator(lineValidator);
    }
}
