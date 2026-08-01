using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Order;

public sealed class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
