using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetByIdAsync(Guid id, Guid userId);

    IQueryable<Order> Filter(
        Guid userId,
        OrderStatus? status,
        Guid? customerId,
        DateOnly? from,
        DateOnly? to);

    Task<Order?> GetWithLinesAsync(Guid id, Guid userId);
}
