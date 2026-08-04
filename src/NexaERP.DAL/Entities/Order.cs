using NexaERP.DAL.Enums;

namespace NexaERP.DAL.Entities;

public class Order : Entity
{
    public Guid CustomerId { get; set; }

    public Guid UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public OrderStatus Status { get; set; }

    public string PaymentMethod { get; set; }

    public string ShippingAddress { get; set; }

    public decimal TotalAmount { get; set; }

    // Navigation property
    public Customer Customer { get; set; } = null!;

    public ICollection<OrderLine> Lines { get; set; }

    public bool IsDeleted { get; set; }
}
