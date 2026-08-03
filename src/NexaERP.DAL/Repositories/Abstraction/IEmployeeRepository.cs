using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IEmployeeRepository
    : IGenericRepository<Employee>
{
    IQueryable<Employee> Filter(
        Guid? departmentId,
        string? search);

    Task<Employee?> GetWithDepartmentAsync(Guid id);

    Task<Employee?> GetByIdAsync(Guid id);
}
