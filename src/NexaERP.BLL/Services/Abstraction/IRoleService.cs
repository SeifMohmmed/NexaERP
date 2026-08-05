using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Roles;

namespace NexaERP.BLL.Services.Abstraction;

public interface IRoleService
{
    Task<IReadOnlyCollection<RoleDto>> GetRolesAsync();

    Task<Result> AssignRoleAsync(
        AssignRoleDto dto);
    Task<Result> RemoveRoleAsync(RemoveRoleDto dto);
    Task<IReadOnlyCollection<RoleDto>> GetUserRolesAsync(Guid userId);

    Task<Result> UpdateUserRolesAsync(
    Guid userId,
    UpdateUserRolesDto dto);
}
