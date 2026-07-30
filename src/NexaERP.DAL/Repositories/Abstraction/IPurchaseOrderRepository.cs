using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IPurchaseOrderRepository :
    IGenericRepository<PurchaseOrder>
{
    IQueryable<PurchaseOrder> Query();

    Task<PurchaseOrder?> GetWithLinesAsync(Guid id);
}
