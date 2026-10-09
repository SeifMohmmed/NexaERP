using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.SalesReport;

public sealed class SalesReportQueryParameters
{
    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public string GroupBy { get; set; } = "day";
}
