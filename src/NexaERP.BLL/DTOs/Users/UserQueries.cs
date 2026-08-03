using System.Linq.Expressions;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.DTOs.Users;

public static class UserQueries
{
    public static Expression<Func<User, UserDto>> ProjectToDto()
    {
        return u => new UserDto
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            CreatedAtUtc = u.CreatedAtUtc,
            UpdatedAtUtc = u.UpdatedAtUtc
        };
    }
}
