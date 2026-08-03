namespace NexaERP.DAL.Entities;

public sealed class User : Entity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    // Keycloak User Id
    public string IdentityId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
