namespace NexaERP.DAL.Entities;

public sealed class Notification : Entity
{
    public Guid UserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? DeliveryStatus { get; set; }

    public User User { get; set; } = null!;
}
