using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class PurchaseOrderRepository(
    ApplicationDbContext context)
    : GenericRepository<PurchaseOrder>(context),
      IPurchaseOrderRepository
{
    public IQueryable<PurchaseOrder> Query(Guid userId)
    {
        return context.PurchaseOrders
            .AsNoTracking()
            .Where(p => p.UserId == userId);
    }

    public async Task<PurchaseOrder?> GetByIdAsync(
        Guid id,
        Guid userId)
    {
        return await context.PurchaseOrders
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);
    }

    public async Task<PurchaseOrder?> GetWithLinesAsync(
        Guid id,
        Guid userId)
    {
        return await context.PurchaseOrders
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p =>
                p.Id == id &&
                p.UserId == userId);
    }
}
