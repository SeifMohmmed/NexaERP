using NexaERP.BLL.DTOs.Common;
using NexaERP.DAL.Enums;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace NexaERP.BLL.DTOs.ProjectTask;

public class ProjectTaskQueryParameters : AcceptHeaderDto
{
    public Guid? ProjectId { get; set; }

    public Guid? AssigneeId { get; set; }

    public ProjectTaskStatus? Status { get; set; }

    public ProjectTaskPriority? Priority { get; set; }

    public DateOnly? DueBefore { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
