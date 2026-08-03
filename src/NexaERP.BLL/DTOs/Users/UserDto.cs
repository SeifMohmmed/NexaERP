using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.Users;

public sealed record UserDto
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public string FullName => $"{FirstName} {LastName}";

    public required string Email { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public List<LinkDto>? Links { get; set; }
}
