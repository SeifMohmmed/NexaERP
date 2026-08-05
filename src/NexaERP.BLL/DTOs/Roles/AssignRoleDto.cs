namespace NexaERP.BLL.DTOs.Roles;

public sealed class AssignRoleDto
{
    public Guid UserId { get; init; }
    public required string Role { get; init; }
}
