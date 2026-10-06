using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.ProjectTask;

public class CreateProjectTaskDto
{
    public Guid ProjectId { get; set; }

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public Guid AssigneeId { get; set; }

    public ProjectTaskPriority Priority { get; set; }

    public DateOnly DueDate { get; set; }

    public ProjectTaskStatus Status { get; set; }
}
