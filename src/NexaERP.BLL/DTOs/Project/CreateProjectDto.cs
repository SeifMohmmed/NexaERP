using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Project;

public sealed class CreateProjectDto
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public Guid OwnerId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly DueDate { get; set; }

    public ProjectStatus Status { get; set; }
}
