using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Context;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    // Returns all categories ordered by name.
    public IQueryable<Category> GetAll()
    {
        return _dbSet
            .AsNoTracking()
            .OrderBy(c => c.Name);
    }
}
