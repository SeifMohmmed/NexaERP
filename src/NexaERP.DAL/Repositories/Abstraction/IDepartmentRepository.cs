using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IDepartmentRepository
    : IGenericRepository<Department>
{
    IQueryable<Department> Query();

}
