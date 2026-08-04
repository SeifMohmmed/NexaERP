using NexaERP.BLL.DTOs.Auth;
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
