using NexaERP.DAL.Enums;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace NexaERP.BLL.DTOs.ProjectTask;

public class UpdateProjectTaskDto
{
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public Guid AssigneeId { get; set; }

    public ProjectTaskPriority Priority { get; set; }

    public DateOnly DueDate { get; set; }

    public ProjectTaskStatus Status { get; set; }
}
