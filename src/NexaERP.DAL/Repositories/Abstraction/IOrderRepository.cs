using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IOrderRepository : IGenericRepository<Order>
{
    IQueryable<Order> Filter(
        OrderStatus? status,
        Guid? customerId,
        DateOnly? from,
        DateOnly? to);

    Task<Order?> GetWithLinesAsync(Guid id);
}
