using FluentValidation;
using NexaERP.BLL.DTOs.Automation;

namespace NexaERP.BLL.Validators.AutomationAction;

public sealed class AutomationActionDtoValidator
    : AbstractValidator<AutomationActionDto>
{
    public AutomationActionDtoValidator()
    {
        RuleFor(action => action.Type)
            .NotEmpty()
            .Must(type =>
                type is "NotifyRole" or "NotifyUser")
            .WithMessage(
                "Action type must be NotifyRole or NotifyUser.");

        When(action => action.Type == "NotifyRole", () =>
        {
            RuleFor(action => action.Role)
                .NotEmpty()
                .WithMessage(
                    "Role is required for NotifyRole.");

            RuleFor(action => action.UserId)
                .Null()
                .WithMessage(
                    "UserId must not be provided for NotifyRole.");
        });

        When(action => action.Type == "NotifyUser", () =>
        {
            RuleFor(action => action.UserId)
                .NotNull()
                .WithMessage(
                    "UserId is required for NotifyUser.");

            RuleFor(action => action.Role)
                .Null()
                .WithMessage(
                    "Role must not be provided for NotifyUser.");
        });
    }
}
