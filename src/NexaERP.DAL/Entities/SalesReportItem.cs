namespace NexaERP.DAL.Entities;

public sealed class SalesReportItem
{
    public string Period { get; set; } = default!;
    public decimal TotalSales { get; set; }
    public int OrderCount { get; set; }
}
