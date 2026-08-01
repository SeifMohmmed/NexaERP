namespace NexaERP.BLL.DTOs.Order;

public sealed class UpdateOrderDto
{
    public Guid CustomerId { get; init; }

    public string PaymentMethod { get; init; }

    public string ShippingAddress { get; init; }
}
