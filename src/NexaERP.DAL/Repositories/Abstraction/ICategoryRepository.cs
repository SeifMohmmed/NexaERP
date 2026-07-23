using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface ICategoryRepository : IGenericRepository<Category>
{
    // Returns all categories.
    IQueryable<Category> GetAll();
}
