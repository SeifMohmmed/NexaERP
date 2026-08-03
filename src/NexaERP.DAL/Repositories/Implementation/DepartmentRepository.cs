using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class DepartmentRepository(
    ApplicationDbContext context)
    : GenericRepository<Department>(context),
      IDepartmentRepository
{
    public IQueryable<Department> Query()
    {
        return _dbSet.AsNoTracking();
    }
}
