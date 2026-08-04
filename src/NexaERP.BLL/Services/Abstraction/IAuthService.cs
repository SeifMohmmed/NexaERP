using NexaERP.BLL.DTOs.Auth;

namespace NexaERP.BLL.Services.Abstraction;

public interface IAuthService
{
    Task<AuthenticationResult> RegisterAsync(RegisterUserDto dto);

    Task<AuthenticationResult> LoginAsync(LoginUserDto dto);

    Task<AuthenticationResult> RefreshAsync(RefreshTokenDto dto);
}
