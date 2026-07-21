using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    IQueryable<Customer> Search(string? search);

}
