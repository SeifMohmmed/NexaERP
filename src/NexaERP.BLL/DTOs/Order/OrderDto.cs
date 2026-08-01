using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.OrderLine;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Order;

public sealed class OrderDto
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public DateTime OrderDate { get; init; }

    public OrderStatus Status { get; init; }

    public string PaymentMethod { get; init; }

    public string ShippingAddress { get; init; }

    public decimal TotalAmount { get; init; }

    public List<OrderLineDto> Lines { get; init; }

    public List<LinkDto> Links { get; set; }
}
