using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class ProjectRepository(
    ApplicationDbContext context)
    : GenericRepository<Project>(context),
        IProjectRepository
{
    public IQueryable<Project> Search(
        string? search,
        ProjectStatus? status,
        Guid? ownerId)
    {
        IQueryable<Project> query = _dbSet
            .AsNoTracking()
            .Include(p => p.Owner);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();

            query = query.Where(p =>
                p.Name.ToLower().Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(p =>
                p.Status == status.Value);
        }

        if (ownerId.HasValue)
        {
            query = query.Where(p =>
                p.OwnerId == ownerId.Value);
        }

        return query;
    }

    public async Task<Project?> GetDetailsByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
