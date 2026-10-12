namespace NexaERP.DAL.Entities;

public sealed class NotificationSettings
{
    public Guid UserId { get; set; }

    public string Channel { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public User User { get; set; } = null!;
}
