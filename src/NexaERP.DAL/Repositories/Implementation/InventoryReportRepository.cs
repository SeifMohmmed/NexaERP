using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class InventoryReportRepository(
    ApplicationDbContext context)
    : IInventoryReportRepository
{
    public async Task<List<InventoryReportItem>> GetInventoryReportAsync(
        Guid? categoryId,
        bool? lowStock,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (categoryId.HasValue)
        {
            query = query.Where(
                p => p.CategoryId == categoryId.Value);
        }

        var products = await query
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.SKU,
                p.CategoryId,
                p.StockQuantity,
                p.ReorderLevel
            })
            .ToListAsync(cancellationToken);

        IEnumerable<InventoryReportItem> result = products
            .Select(p => new InventoryReportItem
            {
                ProductId = p.Id,
                ProductName = p.Name,
                SKU = p.SKU,
                CategoryId = p.CategoryId,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel,
                IsLowStock = p.StockQuantity <= p.ReorderLevel
            });

        if (lowStock.HasValue)
        {
            result = result.Where(
                p => p.IsLowStock == lowStock.Value);
        }

        return result.ToList();
    }
}
