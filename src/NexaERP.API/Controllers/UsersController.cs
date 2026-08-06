using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Users;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Repositories.Abstraction;
using NexaERP.DAL.Services;

namespace NexaERP.API.Controllers;

[Route("users")]
[ApiController]
public sealed class UsersController(
    IUserRepository userRepository,
    UserContext userContext) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.UsersRead)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        Guid? currentUserId = await userContext.GetUserIdAsync();

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        if (id != currentUserId.Value)
        {
            return Forbid();
        }

        var user = await userRepository
            .Query()
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        return user is null ? NotFound() : Ok(user);

    }

    [HttpGet("me")]
    [HasPermission(Permissions.UsersRead)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        Guid? userId = await userContext.GetUserIdAsync();

        if (userId is null)
        {
            return Unauthorized();
        }

        UserDto? user = await userRepository
            .Query()
            .Where(u => u.Id == userId)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("me/permissions")]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetPermissions(
        [FromServices] AuthorizationService authorizationService)
    {
        string identityId = userContext.GetIdentityId();

        if (identityId is null)
        {
            return Unauthorized();
        }

        HashSet<string> permissions =
            await authorizationService.GetPermissionsForUserAsync(identityId);

        return Ok(permissions);
    }
}
