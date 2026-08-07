namespace NexaERP.DAL.Entities;

public sealed class AuditLog
{
    // Audit log identifier.
    public Guid Id { get; set; }

    // Identity user who performed the action.
    public string? IdentityId { get; set; }

    // Name of the affected entity.
    public string EntityName { get; set; }

    // Performed action (Added, Modified, Deleted).
    public string Action { get; set; }

    // Time when the action occurred.
    public DateTime CreatedAtUtc { get; set; }

    // Summary of the property changes.
    public string Changes { get; set; }
}
