using NexaERP.BLL.DTOs.OrderLine;

namespace NexaERP.BLL.DTOs.Order;

public sealed class CreateOrderDto
{
    public Guid CustomerId { get; init; }

    public string PaymentMethod { get; init; }

    public string ShippingAddress { get; init; }

    public List<CreateOrderLineDto> Lines { get; set; }
}
