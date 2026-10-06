using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Project;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.Mappings;

public static class ProjectMapping
{
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName = $"{project.Owner.FirstName} {project.Owner.LastName}",
            StartDate = project.StartDate,
            DueDate = project.DueDate,
            Status = project.Status,
            TotalTasks = project.Tasks.Count,
            CompletedTasks = project.Tasks.Count(t => t.Status == ProjectTaskStatus.Done)
        };
    }

    public static Project ToEntity(this CreateProjectDto dto)
    {
        return new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = dto.OwnerId,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            Status = dto.Status
        };
    }

    public static void UpdateEntity(
        this Project project,
        UpdateProjectDto dto)
    {
        project.Name = dto.Name;
        project.Description = dto.Description;
        project.OwnerId = dto.OwnerId;
        project.StartDate = dto.StartDate;
        project.DueDate = dto.DueDate;
        project.Status = dto.Status;
    }

    public static Expression<Func<Project, ProjectDto>> ProjectToDto()
    {
        return project => new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerName =
                project.Owner.FirstName + " " +
                project.Owner.LastName,
            StartDate = project.StartDate,
            DueDate = project.DueDate,
            Status = project.Status,
            TotalTasks = project.Tasks.Count,
            CompletedTasks =
                project.Tasks.Count(t => t.Status == ProjectTaskStatus.Done)
        };
    }
}
