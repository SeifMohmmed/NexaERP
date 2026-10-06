using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Entities;

public class ProjectTask : Entity
{
    public Guid ProjectId { get; set; }

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public Guid AssigneeId { get; set; }

    public ProjectTaskPriority Priority { get; set; }

    public DateOnly DueDate { get; set; }
    
    public ProjectTaskStatus Status { get; set; }

    //navigation property
    public Project Project { get; set; } = default!;
        
    public User Assignee { get; set; } = default!;

}
