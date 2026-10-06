using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IProjectRepository : IGenericRepository<Project>
{
    IQueryable<Project> Search(
        string? search,
        ProjectStatus? status,
        Guid? ownerId);
    
    Task<Project?> GetDetailsByIdAsync(
        Guid id,
        CancellationToken ct = default);
}
