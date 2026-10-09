namespace NexaERP.BLL.DTOs.ProjectTaskReport;

public sealed class TaskReportQueryParameters
{
    public Guid? ProjectId { get; set; }

    public Guid? AssigneeId { get; set; }

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }
    
    public string? Status { get; set; }
}
