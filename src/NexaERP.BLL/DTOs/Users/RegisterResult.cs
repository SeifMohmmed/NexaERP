namespace NexaERP.BLL.DTOs.Users;

/// <summary>
/// Represents the result of a user registration.
/// </summary>
public sealed record RegisterResult
{
    // Indicates whether the registration succeeded.
    public bool Succeeded { get; init; }

    // Identifier of the created user.
    public Guid? UserId { get; init; }

    // Validation or registration errors.
    public Dictionary<string, string>? Errors { get; init; }
}
