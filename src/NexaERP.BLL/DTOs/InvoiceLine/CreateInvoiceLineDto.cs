namespace NexaERP.BLL.DTOs.InvoiceLine;

public sealed class CreateInvoiceLineDto
{
    public string Description { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal TaxRate { get; init; }
}
