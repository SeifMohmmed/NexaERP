namespace NexaERP.DAL.Entities;

public class InvoiceLine : Entity
{
    public Guid InvoiceId { get; set; }

    public string Description { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TaxRate { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation Properties
    public Invoice Invoice { get; set; }
}
