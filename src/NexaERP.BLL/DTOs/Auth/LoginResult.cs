namespace NexaERP.BLL.DTOs.Auth;

public sealed record LoginResult
{
    public bool Succeeded { get; init; }

    public AccessTokenDto? Token { get; init; }

    public Dictionary<string, string>? Errors { get; init; }
}
