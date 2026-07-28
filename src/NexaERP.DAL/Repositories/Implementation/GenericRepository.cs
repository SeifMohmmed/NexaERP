using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal class GenericRepository<T> : IGenericRepository<T>
    where T : Entity
{
    // Entity DbSet.
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    // Adds a new entity.
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    // Adds multiple entities.
    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    // Returns the entity count.
    public Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate is null
            ? _dbSet.CountAsync(ct)
            : _dbSet.CountAsync(predicate, ct);
    }

    // Soft deletes if supported; otherwise removes the entity.
    public void Delete(T entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.IsDeleted = true;
            Update(entity);
            return;
        }

        _dbSet.Remove(entity);
    }

    // Deletes multiple entities.
    public void DeleteRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Delete(entity);
        }
    }

    // Checks whether an entity exists.
    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(predicate, ct);
    }

    // Returns all matching entities.
    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync(ct);
    }

    // Returns an entity by ID.
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    // Updates an entity.
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    // Updates multiple entities.
    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
}
