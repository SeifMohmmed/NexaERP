using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Users;
using NexaERP.BLL.Services.Abstraction;

namespace NexaERP.API.Controllers;

[Route("auth")]
[ApiController]
[AllowAnonymous]
public sealed class AuthController(
    IAuthService authService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserDto dto,
        [FromServices] IValidator<RegisterUserDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var result = await authService.RegisterAsync(dto);

        if (!result.Succeeded)
        {
            return Problem(
                detail: "Unable to register user, please try again",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    { "errors", result.Errors }
                });
        }

        return Ok(result.UserId);
    }
}
