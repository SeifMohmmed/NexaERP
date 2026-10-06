using System.Linq.Expressions;
using NexaERP.BLL.DTOs.ProjectTask;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class ProjectTaskMapping
{
    public static ProjectTaskDto ToDto(this ProjectTask task)
    {
        return new ProjectTaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            Title = task.Title,
            Description = task.Description,
            AssigneeId = task.AssigneeId,
            AssigneeName =
                $"{task.Assignee.FirstName} {task.Assignee.LastName}",
            Priority = task.Priority,
            DueDate = task.DueDate,
            Status = task.Status,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ProjectTask ToEntity(this CreateProjectTaskDto dto)
    {
        return new ProjectTask
        {
            ProjectId = dto.ProjectId,
            Title = dto.Title,
            Description = dto.Description,
            AssigneeId = dto.AssigneeId,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            Status = dto.Status
        };
    }

    public static void UpdateEntity(
        this ProjectTask task,
        UpdateProjectTaskDto dto)
    {
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.AssigneeId = dto.AssigneeId;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.Status = dto.Status;
    }

    public static Expression<Func<ProjectTask, ProjectTaskDto>> ProjectToDto()
    {
        return task => new ProjectTaskDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            Title = task.Title,
            Description = task.Description,
            AssigneeId = task.AssigneeId,
            AssigneeName =
                task.Assignee.FirstName + " " +
                task.Assignee.LastName,
            Priority = task.Priority,
            DueDate = task.DueDate,
            Status = task.Status,
            CreatedAt = DateTime.UtcNow
        };
    }
}
