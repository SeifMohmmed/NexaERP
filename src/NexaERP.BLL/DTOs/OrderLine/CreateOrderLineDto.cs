namespace NexaERP.BLL.DTOs.OrderLine;

public sealed class CreateOrderLineDto
{
    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Discount { get; init; }
}
