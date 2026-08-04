namespace NexaERP.BLL.DTOs.Auth;

public sealed record RefreshTokenDto
{
    public required string RefreshToken { get; init; }  // Refresh token string.
}
