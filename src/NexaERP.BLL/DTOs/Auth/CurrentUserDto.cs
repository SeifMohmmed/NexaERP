namespace NexaERP.BLL.DTOs.Auth;

public sealed class CurrentUserDto
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IList<string> Roles { get; set; } = [];
}
