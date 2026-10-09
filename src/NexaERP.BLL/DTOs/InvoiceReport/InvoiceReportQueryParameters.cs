namespace NexaERP.BLL.DTOs.InvoiceReport;

public sealed class InvoiceReportQueryParameters
{
    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public string? Status { get; set; }
}
