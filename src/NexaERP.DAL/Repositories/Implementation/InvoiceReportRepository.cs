using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class InvoiceReportRepository(
    ApplicationDbContext context)
    : IInvoiceReportRepository
{
    public async Task<List<InvoiceReportItem>> GetInvoiceReportAsync(
        DateOnly? from,
        DateOnly? to,
        string? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = context.Invoices
            .AsNoTracking()
            .Where(i => !i.IsDeleted);

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(
                from.Value.ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            query = query.Where(i => i.InvoiceDate >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(
                to.Value.ToDateTime(TimeOnly.MaxValue),
                DateTimeKind.Utc);

            query = query.Where(i => i.InvoiceDate <= toUtc);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            InvoiceStatus invoiceStatus =
                Enum.Parse<InvoiceStatus>(
                    status,
                    ignoreCase: true);

            query = query.Where(i => i.Status == invoiceStatus);
        }

        var invoices = await query
            .Select(i => new
            {
                i.Id,
                i.InvoiceDate,
                i.DueDate,
                i.Status,
                i.TotalAmount,
                i.PaidAt
            })
            .ToListAsync(cancellationToken);

        DateTime nowUtc = DateTime.UtcNow;

        return invoices
            .Select(i =>
            {
                bool isOverdue =
                    i.Status != InvoiceStatus.Paid &&
                    i.DueDate < nowUtc;

                int overdueDays = isOverdue
                    ? (nowUtc.Date - i.DueDate.Date).Days
                    : 0;

                return new InvoiceReportItem
                {
                    InvoiceId = i.Id,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    Status = i.Status.ToString(),
                    TotalAmount = i.TotalAmount,
                    IsOverdue = isOverdue,
                    OverdueDays = overdueDays
                };
            })
            .ToList();
    }
}
