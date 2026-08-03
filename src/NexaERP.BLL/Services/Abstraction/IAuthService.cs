using NexaERP.BLL.DTOs.Auth;
using NexaERP.BLL.DTOs.Users;

namespace NexaERP.BLL.Services.Abstraction;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterUserDto dto);

    Task<LoginResult> LoginAsync(LoginUserDto dto);

}
