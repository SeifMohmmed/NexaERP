using FluentValidation;
using NexaERP.BLL.DTOs.Product;

namespace NexaERP.BLL.DTOs.Validators.Product;

internal sealed class AdjustStockDtoValidator
    : AbstractValidator<AdjustStockDto>
{
    public AdjustStockDtoValidator()
    {
        // Quantity must not be zero.
        RuleFor(x => x.QuantityChange)
            .NotEqual(0);

        // Reason is required and limited in length.
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}
