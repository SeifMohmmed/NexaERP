namespace NexaERP.BLL.DTOs.Invoice;

public sealed class PayInvoiceDto
{
    public DateTime PaidAt { get; init; }

    public string PaymentMethod { get; init; }
}
