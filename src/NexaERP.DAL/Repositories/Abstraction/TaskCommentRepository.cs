using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface ITaskCommentRepository
    : IGenericRepository<TaskComment>
{
    IQueryable<TaskComment> GetByTaskId(Guid taskId);

    Task<TaskComment?> GetDetailsByIdAsync(
        Guid id,
        CancellationToken ct = default);
}
