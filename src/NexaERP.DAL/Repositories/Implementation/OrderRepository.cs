using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class OrderRepository(
    ApplicationDbContext context)
    : GenericRepository<Order>(context),
      IOrderRepository
{
    public IQueryable<Order> Filter(
        OrderStatus? status,
        Guid? customerId,
        DateOnly? from,
        DateOnly? to)
    {
        var query = context.Orders
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (customerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == customerId.Value);
        }

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(
                from.Value.ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            query = query.Where(o => o.OrderDate >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(
                to.Value.ToDateTime(TimeOnly.MaxValue),
                DateTimeKind.Utc);

            query = query.Where(o => o.OrderDate <= toUtc);
        }

        return query;
    }

    public async Task<Order?> GetWithLinesAsync(Guid id)
    {
        return await context.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
