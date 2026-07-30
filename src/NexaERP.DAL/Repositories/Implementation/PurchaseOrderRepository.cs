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
    public async Task<PurchaseOrder?> GetWithLinesAsync(Guid id)
    {
        return await context.PurchaseOrders
            .Include(po => po.Lines)
            .FirstOrDefaultAsync(po => po.Id == id);
    }

    public IQueryable<PurchaseOrder> Query()
    {
        return context.PurchaseOrders
            .AsNoTracking();
    }
}
