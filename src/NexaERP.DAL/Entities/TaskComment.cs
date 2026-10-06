namespace NexaERP.DAL.Entities;

public sealed class TaskComment : Entity
{
    public Guid TaskId { get; set; }

    public Guid AuthorId { get; set; }

    public string Body { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ProjectTask Task { get; set; } = default!;

    public User Author { get; set; } = default!;
}
