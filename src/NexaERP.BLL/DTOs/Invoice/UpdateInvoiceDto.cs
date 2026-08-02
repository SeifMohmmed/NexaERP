namespace NexaERP.BLL.DTOs.Invoice;

public sealed class UpdateInvoiceDto
{
    public Guid CustomerId { get; init; }

    public DateTime InvoiceDate { get; init; }

    public DateTime DueDate { get; init; }
}
