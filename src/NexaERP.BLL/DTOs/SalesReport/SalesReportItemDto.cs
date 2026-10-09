using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.SalesReport;

public sealed class SalesReportItemDto 
{
    public string Period { get; set; } = default!;
    public decimal TotalSales { get; set; }
    public int OrderCount { get; set; }
    public string Currency { get; set; } = default!;

}
