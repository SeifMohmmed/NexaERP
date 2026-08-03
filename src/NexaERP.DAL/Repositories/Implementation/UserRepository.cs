using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class UserRepository(
    ApplicationDbContext context)
    : GenericRepository<User>(context),
      IUserRepository
{
    public IQueryable<User> Query()
    {
        return _dbSet.AsNoTracking();
    }
}
