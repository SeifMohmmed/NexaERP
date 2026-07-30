namespace NexaERP.DAL.Entities;

public class PurchaseLine : Entity
{
    public Guid PurchaseOrderId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    // Navigation properties
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
