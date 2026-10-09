using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface ISalesReportRepository
{
    Task<List<SalesReportItem>> GetSalesReportAsync(
        DateOnly? from,
        DateOnly? to,
        string groupBy,
        CancellationToken cancellationToken = default);
}
