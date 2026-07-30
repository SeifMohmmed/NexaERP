namespace NexaERP.DAL.Entities;

public class PurchaseOrder : Entity
{
    public Guid SupplierId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ExpectedDelivery { get; set; }

    public PurchaseOrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    // Navigation property
    public Supplier Supplier { get; set; }

    public ICollection<PurchaseLine> Lines { get; set; }

}
