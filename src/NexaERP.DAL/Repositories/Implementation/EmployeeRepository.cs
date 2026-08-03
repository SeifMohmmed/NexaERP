using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class EmployeeRepository(
    ApplicationDbContext context)
    : GenericRepository<Employee>(context),
      IEmployeeRepository
{
    public IQueryable<Employee> Filter(
        Guid? departmentId,
        string? search)
    {
        var query = context.Employees
            .AsNoTracking()
            .AsQueryable()
            .Where(e => !e.IsDeleted);

        if (departmentId.HasValue)
        {
            query = query.Where(e =>
                e.DepartmentId == departmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.FirstName.Contains(search) ||
                e.LastName.Contains(search) ||
                e.Email.Contains(search));
        }

        return query;
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await context.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetWithDepartmentAsync(Guid id)
    {
        return await context.Employees
            .Where(e => !e.IsDeleted)
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
