using FluentValidation;
using NexaERP.BLL.DTOs.PurchaseLine;

namespace NexaERP.BLL.DTOs.Validators.PurchaseLine;

public sealed class CreatePurchaseLineDtoValidator
    : AbstractValidator<CreatePurchaseLineDto>
{
    public CreatePurchaseLineDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0);
    }
}
