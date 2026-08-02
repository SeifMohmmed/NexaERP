using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Entities;

public class Invoice : Entity, ISoftDeletable
{
    public Guid? OrderId { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public InvoiceStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? PaymentMethod { get; set; }

    // Navigation Properties
    public Order? Order { get; set; }

    public Customer Customer { get; set; }

    public ICollection<InvoiceLine> Lines { get; set; }
}
