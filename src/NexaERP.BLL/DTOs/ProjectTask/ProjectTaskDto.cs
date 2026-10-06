using NexaERP.BLL.DTOs.Common;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.ProjectTask;

public class ProjectTaskDto
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string ProjectName { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public Guid AssigneeId { get; set; }

    public string AssigneeName { get; set; } = default!;

    public ProjectTaskPriority Priority { get; set; }

    public DateOnly DueDate { get; set; }

    public ProjectTaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public List<LinkDto>? Links { get; set; }
}
