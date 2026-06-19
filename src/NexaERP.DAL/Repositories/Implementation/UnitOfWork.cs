using NexaERP.DAL.Context;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

public sealed class UnitOfWork(
    ApplicationDbContext context) : IUnitOfWork
{

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await context.SaveChangesAsync(ct);
    }
}
