using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    IQueryable<Supplier> Query();
}
