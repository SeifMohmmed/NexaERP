namespace NexaERP.BLL.DTOs.ProjectTaskReport;

public sealed class TaskReportItemDto
{
    public Guid? ProjectId { get; set; }

    public Guid? AssigneeId { get; set; }

    public string Status { get; set; } = default!;

    public int TaskCount { get; set; }
}
