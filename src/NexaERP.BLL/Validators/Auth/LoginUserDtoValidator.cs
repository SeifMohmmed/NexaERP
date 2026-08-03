using FluentValidation;
using NexaERP.BLL.DTOs.Auth;

namespace NexaERP.BLL.Validators.Auth;

internal sealed class LoginUserDtoValidator
    : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
