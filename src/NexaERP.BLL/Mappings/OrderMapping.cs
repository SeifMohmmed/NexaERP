using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Order;
using NexaERP.BLL.DTOs.OrderLine;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.Mappings;

public static class OrderMapping
{
    public static Order ToEntity(this CreateOrderDto dto)
    {
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            PaymentMethod = dto.PaymentMethod,
            ShippingAddress = dto.ShippingAddress,

            Lines = dto.Lines
                .Select(line => line.ToEntity())
                .ToList()
        };

        order.TotalAmount = order.Lines.Sum(line =>
            line.Quantity * line.UnitPrice - line.Discount);

        return order;
    }

    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            ShippingAddress = order.ShippingAddress,
            TotalAmount = order.TotalAmount,

            Lines = order.Lines
                .Select(line => line.ToDto())
                .ToList()
        };
    }

    public static void UpdateEntity(
        this Order order,
        UpdateOrderDto dto)
    {
        order.CustomerId = dto.CustomerId;
        order.PaymentMethod = dto.PaymentMethod;
        order.ShippingAddress = dto.ShippingAddress;
    }

    public static Expression<Func<Order, OrderDto>> ProjectToDto()
    {
        return order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            ShippingAddress = order.ShippingAddress,
            TotalAmount = order.TotalAmount,

            Lines = order.Lines
                .Select(line => new OrderLineDto
                {
                    Id = line.Id,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    Discount = line.Discount
                })
                .ToList()
        };
    }
}
