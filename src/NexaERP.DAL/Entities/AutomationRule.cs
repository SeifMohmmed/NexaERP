namespace NexaERP.DAL.Entities;

public sealed class AutomationRule : Entity
{
    public string Name { get; set; } = string.Empty;

    public string Trigger { get; set; } = string.Empty;

    // Stores the action configuration as JSON.
    public string Action { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    // Keycloak identity ID of the user who created the rule.
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<AutomationRun> Runs { get; set; }
        = new List<AutomationRun>();
}
