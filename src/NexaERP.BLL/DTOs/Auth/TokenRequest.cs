namespace NexaERP.BLL.DTOs.Auth;

public sealed record TokenRequest(
    string UserId,
    string Email,
    IEnumerable<string> Roles);
