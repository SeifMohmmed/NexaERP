using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class TaskCommentRepository(
    ApplicationDbContext context)
    : GenericRepository<TaskComment>(context),
        ITaskCommentRepository
{
    public IQueryable<TaskComment> GetByTaskId(Guid taskId)
    {
        return _dbSet
            .AsNoTracking()
            .Where(c => c.TaskId == taskId)
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt);
    }

    public async Task<TaskComment?> GetDetailsByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Include(c => c.Author)
            .Include(c => c.Task)
            .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}
