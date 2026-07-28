using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context)
    : base(context)
    {
    }

    public IQueryable<Supplier> Query()
    {
        return _dbSet.AsNoTracking();
    }
}
