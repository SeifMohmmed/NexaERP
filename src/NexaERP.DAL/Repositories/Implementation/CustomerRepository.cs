using Microsoft.EntityFrameworkCore;
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

    // Searches customers by common fields.
    public IQueryable<Customer> Search(string? search)
    {
        // Base query without change tracking.
        IQueryable<Customer> query = _dbSet.AsNoTracking();

        // Return all customers if no search term is provided.
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        search = search.Trim();

        // Filter customers by matching searchable fields.
        return query.Where(c =>
            c.Name.ToLower().Contains(search) ||
            c.Email.ToLower().Contains(search) ||
            c.Phone.ToLower().Contains(search) ||
            c.City.ToLower().Contains(search) ||
            c.Country.ToLower().Contains(search));
    }
}
