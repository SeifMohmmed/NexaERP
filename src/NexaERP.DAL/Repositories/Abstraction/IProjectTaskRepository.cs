using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IProjectTaskRepository
    : IGenericRepository<ProjectTask>
{
    IQueryable<ProjectTask> Search(
        Guid? projectId,
        Guid? assigneeId,
        ProjectTaskStatus? status,
        ProjectTaskPriority? priority,
        DateOnly? dueBefore);
    
    Task<ProjectTask?> GetDetailsByIdAsync(
        Guid id,
        CancellationToken ct = default);
}
