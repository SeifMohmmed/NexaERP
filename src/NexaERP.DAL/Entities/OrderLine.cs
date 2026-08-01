namespace NexaERP.DAL.Entities;

public class OrderLine : Entity
{
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
