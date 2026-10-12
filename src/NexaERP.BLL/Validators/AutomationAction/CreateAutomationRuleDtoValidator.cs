using FluentValidation;
using NexaERP.BLL.DTOs.Automation;

namespace NexaERP.BLL.Validators.AutomationAction;

public sealed class CreateAutomationRuleDtoValidator
    : AbstractValidator<CreateAutomationRuleDto>
{
    public CreateAutomationRuleDtoValidator(
        IValidator<AutomationActionDto> actionValidator)
    {
        RuleFor(rule => rule.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(rule => rule.Trigger)
            .NotEmpty()
            .Must(trigger =>
                trigger is "InventoryBelowReorderLevel"
                    or "InvoiceOverdue")
            .WithMessage(
                "Trigger must be InventoryBelowReorderLevel or InvoiceOverdue.");

        RuleFor(rule => rule.Action)
            .NotNull()
            .SetValidator(actionValidator);
    }
}
