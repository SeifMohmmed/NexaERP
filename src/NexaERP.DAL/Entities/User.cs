using Microsoft.AspNetCore.Identity;

namespace NexaERP.DAL.Entities;

public sealed class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public static string CreateNewId() => $"e_{Guid.CreateVersion7()}";
}
