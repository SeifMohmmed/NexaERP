using NexaERP.BLL.DTOs.Users;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class UserMappings
{
    public static User ToEntity(this RegisterUserDto dto)
    {
        return new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
