using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class ProjectTaskRepository(
    ApplicationDbContext context)
    : GenericRepository<ProjectTask>(context),
        IProjectTaskRepository
{
    public IQueryable<ProjectTask> Search(
        Guid? projectId,
        Guid? assigneeId,
        ProjectTaskStatus? status,
        ProjectTaskPriority? priority,
        DateOnly? dueBefore)
    {
        IQueryable<ProjectTask> query = _dbSet
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Assignee);

        if (projectId.HasValue)
        {
            query = query.Where(t =>
                t.ProjectId == projectId.Value);
        }

        if (assigneeId.HasValue)
        {
            query = query.Where(t =>
                t.AssigneeId == assigneeId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(t =>
                t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t =>
                t.Priority == priority.Value);
        }

        if (dueBefore.HasValue)
        {
            query = query.Where(t =>
                t.DueDate <= dueBefore.Value);
        }

        return query;
    }

    public async Task<ProjectTask?> GetDetailsByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }
}
