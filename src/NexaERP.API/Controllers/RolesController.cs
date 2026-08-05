using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Roles;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Identity;

namespace NexaERP.API.Controllers;

[Authorize(Roles = Roles.Admin)]
[Route("roles")]
[ApiController]
public sealed class RolesController(
    IRoleService roleService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetRoles()
    {
        IReadOnlyCollection<RoleDto> roles =
            await roleService.GetRolesAsync();

        return Ok(roles);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(
    AssignRoleDto dto)
    {
        Result result =
            await roleService.AssignRoleAsync(dto);

        if (!result.Succeeded)
        {
            return Problem(
                detail: "Unable to assign role.",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    ["errors"] = result.Errors
                });
        }

        return NoContent();
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetUserRoles(
    Guid userId)
    {
        IReadOnlyCollection<RoleDto> roles =
            await roleService.GetUserRolesAsync(userId);

        return Ok(roles);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveRole(
    RemoveRoleDto dto)
    {
        Result result =
            await roleService.RemoveRoleAsync(dto);

        if (!result.Succeeded)
        {
            return Problem(
                detail: "Unable to remove role.",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    ["errors"] = result.Errors
                });
        }

        return NoContent();
    }

    [HttpPut("users/{userId:guid}")]
    public async Task<IActionResult> UpdateUserRoles(
    Guid userId,
    UpdateUserRolesDto dto)
    {
        Result result =
            await roleService.UpdateUserRolesAsync(
                userId,
                dto);

        if (!result.Succeeded)
        {
            return Problem(
                detail: "Unable to update user roles.",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    ["errors"] = result.Errors
                });
        }

        return NoContent();
    }
}
