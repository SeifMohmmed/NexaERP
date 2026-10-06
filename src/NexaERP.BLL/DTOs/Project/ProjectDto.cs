using NexaERP.BLL.DTOs.Common;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Project;

public sealed class ProjectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public Guid OwnerId { get; set; }

    public string OwnerName { get; set; } = default!;

    public DateOnly StartDate { get; set; }

    public DateOnly DueDate { get; set; }

    public ProjectStatus Status { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }
    
    public List<LinkDto>? Links { get; set; }

}
