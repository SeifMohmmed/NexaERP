namespace NexaERP.DAL.Repositories.Abstraction;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(
    CancellationToken ct = default);
}
