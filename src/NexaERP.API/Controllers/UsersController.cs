using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Users;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Route("users")]
[ApiController]
public sealed class UsersController(
    IUserRepository userRepository) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await userRepository
            .Query()
            .Where(u => u.Id == id)
            .Select(UserQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

}
