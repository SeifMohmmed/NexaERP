using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface ITaskReportRepository
{
    Task<List<TaskReportItem>> GetTaskReportAsync(
        Guid? projectId,
        Guid? assigneeId,
        DateOnly? from,
        DateOnly? to,
        string? status,
        CancellationToken cancellationToken = default);
}
