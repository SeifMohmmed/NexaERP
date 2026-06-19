using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Context;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{

    private readonly DbSet<T> _dbSet;
    public GenericRepository(
        ApplicationDbContext context)
    {
        _dbSet = context.Set<T>();
    }
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate is null
            ? _dbSet.CountAsync(ct)
            : _dbSet.CountAsync(
                predicate,
                ct);
    }

    public void Delete(
        T entity)
    {
        if (typeof(BaseEntity)
            .IsAssignableFrom(typeof(T)))
        {
            entity.IsDeleted = true;

            Update(entity);

            return;
        }

        _dbSet.Remove(entity);
    }

    public void DeleteRange(
        IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Delete(entity);
        }
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _dbSet
            .AnyAsync(predicate, ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
}
