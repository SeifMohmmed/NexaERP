namespace NexaERP.BLL.DTOs.Roles;

public sealed class RemoveRoleDto
{
    public Guid UserId { get; init; }

    public required string Role { get; init; }
}
