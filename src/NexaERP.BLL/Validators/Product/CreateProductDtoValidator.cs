using FluentValidation;
using NexaERP.BLL.DTOs.Product;

namespace NexaERP.BLL.Validators.Product;

internal sealed class CreateProductDtoValidator
    : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        // Validate product name.
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        // Validate SKU.
        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(50);

        // Category is required.
        RuleFor(x => x.CategoryId)
            .NotEmpty();

        // Prices cannot be negative.
        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0);

        // Stock values cannot be negative.
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0);
    }
}
