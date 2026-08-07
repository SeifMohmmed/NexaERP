using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    // Filters products by category and stock level.
    public IQueryable<Product> Filter(
        Guid? categoryId,
        bool? lowStock)
    {
        IQueryable<Product> products = _dbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted);

        // Filter by category.
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        // Return only low-stock products.
        if (lowStock is true)
        {
            products = products.Where(p => p.StockQuantity <= p.ReorderLevel);
        }

        return products;
    }

    // Returns a product with its category.
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }
}
