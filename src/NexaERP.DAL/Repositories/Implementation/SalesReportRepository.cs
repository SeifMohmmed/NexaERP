using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Repositories.Implementation;

internal sealed class SalesReportRepository(
    ApplicationDbContext context)
    : ISalesReportRepository
{
    public async Task<List<SalesReportItem>> GetSalesReportAsync(
        DateOnly? from,
        DateOnly? to,
        string groupBy,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = context.Orders
            .AsNoTracking()
            .Where(o => !o.IsDeleted);

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(
                from.Value.ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            query = query.Where(o => o.OrderDate >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(
                to.Value.ToDateTime(TimeOnly.MaxValue),
                DateTimeKind.Utc);

            query = query.Where(o => o.OrderDate <= toUtc);
        }

        var orders = await query
            .Select(o => new
            {
                o.OrderDate,
                o.TotalAmount
            })
            .ToListAsync(cancellationToken);

        return orders
            .GroupBy(o => GetPeriod(o.OrderDate, groupBy))
            .OrderBy(g => g.Key)
            .Select(g => new SalesReportItem
            {
                Period = g.Key,
                TotalSales = g.Sum(x => x.TotalAmount),
                OrderCount = g.Count()
            })
            .ToList();
    }

    private static string GetPeriod(
        DateTime orderDate,
        string groupBy)
    {
        return groupBy.ToLowerInvariant() switch
        {
            "day" => orderDate.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture),

            "week" => GetWeekPeriod(orderDate),

            "month" => orderDate.ToString(
                "yyyy-MM",
                CultureInfo.InvariantCulture),

            _ => throw new ArgumentException(
                "Invalid groupBy value.",
                nameof(groupBy))
        };
    }

    private static string GetWeekPeriod(DateTime date)
    {
        int week = ISOWeek.GetWeekOfYear(date);
        int year = ISOWeek.GetYear(date);

        return $"{year}-W{week:D2}";
    }
}
