using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IPurchaseOrderRepository :
    IGenericRepository<PurchaseOrder>
{
    IQueryable<PurchaseOrder> Query(Guid userId);

    Task<PurchaseOrder?> GetByIdAsync(Guid id, Guid userId);

    Task<PurchaseOrder?> GetWithLinesAsync(Guid id, Guid userId);
}
