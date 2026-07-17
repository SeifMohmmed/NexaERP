using NexaERP.DAL.Context;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class CustomerRepository
    : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context)
    : base(context)
    {
    }
}
