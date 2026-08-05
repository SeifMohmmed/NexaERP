namespace NexaERP.BLL.DTOs.Roles;

public sealed class UpdateUserRolesDto
{
    public required IReadOnlyCollection<string> Roles { get; init; }
}
