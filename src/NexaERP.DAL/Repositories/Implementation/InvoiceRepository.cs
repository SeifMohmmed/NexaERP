using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class InvoiceRepository(
    ApplicationDbContext context)
    : GenericRepository<Invoice>(context),
      IInvoiceRepository
{
    public IQueryable<Invoice> Filter(
        InvoiceStatus? status,
        Guid? customerId,
        DateTime? from,
        DateTime? to)
    {
        var query = context.Invoices
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(i => i.CustomerId == customerId.Value);
        }

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(
                from.Value,
                DateTimeKind.Utc);

            query = query.Where(i => i.InvoiceDate >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(
                to.Value,
                DateTimeKind.Utc);

            query = query.Where(i => i.InvoiceDate <= toUtc);
        }

        return query;
    }

    public async Task<Invoice?> GetWithLinesAsync(Guid id)
    {
        return await context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == id);
    }
}
