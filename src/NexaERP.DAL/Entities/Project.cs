using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Entities;

public sealed class Project : Entity, ISoftDeletable
{
    // Project name.
    public string Name { get; set; } = default!;

    // Project description.
    public string? Description { get; set; }

    // User who owns the project.
    public Guid OwnerId { get; set; }

    // Project start date.
    public DateOnly StartDate { get; set; }

    // Project due date.
    public DateOnly DueDate { get; set; }

    // Current project status.
    public ProjectStatus Status { get; set; }

    // Indicates whether the project is soft deleted.
    public bool IsDeleted { get; set; }

    // Related tasks.
    public ICollection<ProjectTask> Tasks { get; set; } = [];

    //Related users
    public User Owner { get; set; } = default!;
}
