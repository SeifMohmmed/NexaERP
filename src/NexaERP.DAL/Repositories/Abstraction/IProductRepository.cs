using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IProductRepository : IGenericRepository<Product>
{
    // Filters products by category and stock level.
    IQueryable<Product> Filter(Guid? categoryId, bool? lowStock);

    // Returns a product with its category.
    Task<Product?> GetByIdAsync(Guid id);
}
