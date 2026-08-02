using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.InvoiceLine;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Invoice;

public sealed class InvoiceDto
{
    public Guid Id { get; init; }

    public Guid? OrderId { get; init; }

    public Guid CustomerId { get; init; }

    // Customer Information
    public string CustomerName { get; init; }

    public string CustomerEmail { get; init; }

    public string CustomerPhone { get; init; }

    public string CustomerAddress { get; init; }

    public DateTime InvoiceDate { get; init; }

    public DateTime DueDate { get; init; }

    public InvoiceStatus Status { get; init; }

    public decimal TotalAmount { get; init; }

    public DateTime? PaidAt { get; init; }

    public string? PaymentMethod { get; init; }

    public List<InvoiceLineDto> Lines { get; init; }

    public List<LinkDto> Links { get; set; }
}
