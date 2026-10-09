using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Repositories.Abstraction;

public interface IInvoiceReportRepository
{
    Task<List<InvoiceReportItem>> GetInvoiceReportAsync(
        DateOnly? from,
        DateOnly? to,
        string? status,
        CancellationToken cancellationToken = default);
}
