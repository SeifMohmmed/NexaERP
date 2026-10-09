using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class TaskReportRepository(
    ApplicationDbContext context)
    : ITaskReportRepository
{
    public async Task<List<TaskReportItem>> GetTaskReportAsync(
        Guid? projectId,
        Guid? assigneeId,
        DateOnly? from,
        DateOnly? to,
        string? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ProjectTask> query = context.ProjectTasks
            .AsNoTracking();

        if (projectId.HasValue)
        {
            query = query.Where(
                task => task.ProjectId == projectId.Value);
        }

        if (assigneeId.HasValue)
        {
            query = query.Where(
                task => task.AssigneeId == assigneeId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(
                task => task.DueDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(
                task => task.DueDate <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            ProjectTaskStatus taskStatus =
                Enum.Parse<ProjectTaskStatus>(
                    status,
                    ignoreCase: true);

            query = query.Where(
                task => task.Status == taskStatus);
        }
        
        return await query
            .GroupBy(task => new
            {
                task.ProjectId,
                task.AssigneeId,
                task.Status
            })
            .Select(group => new TaskReportItem
            {
                ProjectId = group.Key.ProjectId,
                AssigneeId = group.Key.AssigneeId,
                Status = group.Key.Status.ToString(),
                TaskCount = group.Count()
            })
            .OrderBy(item => item.ProjectId)
            .ThenBy(item => item.AssigneeId)
            .ThenBy(item => item.Status)
            .ToListAsync(cancellationToken);
    }
}
