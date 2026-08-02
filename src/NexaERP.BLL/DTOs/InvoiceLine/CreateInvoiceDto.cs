namespace NexaERP.BLL.DTOs.InvoiceLine;

public sealed class CreateInvoiceDto
{
    public Guid CustomerId { get; init; }

    public DateTime InvoiceDate { get; init; }

    public DateTime DueDate { get; init; }

    public List<CreateInvoiceLineDto> Lines { get; init; }
}
