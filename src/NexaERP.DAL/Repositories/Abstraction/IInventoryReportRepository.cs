using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IInventoryReportRepository
{
    Task<List<InventoryReportItem>> GetInventoryReportAsync(
        Guid? categoryId,
        bool? lowStock,
        CancellationToken cancellationToken = default);
}
