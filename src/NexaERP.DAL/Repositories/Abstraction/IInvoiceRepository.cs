using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    IQueryable<Invoice> Filter(
        InvoiceStatus? status,
        Guid? customerId,
        DateTime? from,
        DateTime? to);

    Task<Invoice?> GetWithLinesAsync(Guid id);
}
