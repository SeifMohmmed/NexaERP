namespace NexaERP.BLL.DTOs.OrderLine;

public sealed class OrderLineDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }
}
