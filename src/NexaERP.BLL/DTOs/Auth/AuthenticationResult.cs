using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.Auth;

public class AuthenticationResult : Result
{
    public AccessTokenDto? Token { get; init; }
}
