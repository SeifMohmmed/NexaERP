using FluentValidation;
using NexaERP.BLL.DTOs.Auth;

namespace NexaERP.BLL.Validators.Auth;

internal sealed class RefreshTokenDtoValidator
    : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
