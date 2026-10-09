namespace NexaERP.DAL.Entities;

public sealed class InvoiceReportItem
{
    public Guid InvoiceId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = default!;

    public decimal TotalAmount { get; set; }

    public bool IsOverdue { get; set; }

    public int OverdueDays { get; set; }
}
