using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IUserRepository : IGenericRepository<User>
{
    IQueryable<User> Query();

}
