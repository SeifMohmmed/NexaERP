namespace NexaERP.DAL.Entities;

public class BackgroundJob : Entity
{
    public string Type { get; set; } = string.Empty;

    public string RequestedBy { get; set; } = string.Empty;

    public string Status { get; set; } = "Queued";

    public string? Parameters { get; set; }

    public string? ResultLocation { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? HangfireJobId { get; set; }
}   
