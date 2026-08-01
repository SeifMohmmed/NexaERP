using NexaERP.BLL.DTOs.OrderLine;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class OrderLineMapping
{
    public static OrderLine ToEntity(this CreateOrderLineDto dto)
    {
        return new OrderLine
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            Discount = dto.Discount
        };
    }

    public static OrderLineDto ToDto(this OrderLine orderLine)
    {
        return new OrderLineDto
        {
            Id = orderLine.Id,
            ProductId = orderLine.ProductId,
            Quantity = orderLine.Quantity,
            UnitPrice = orderLine.UnitPrice,
            Discount = orderLine.Discount
        };
    }
}
