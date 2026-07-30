using NexaERP.DAL.Entities;

namespace NexaERP.BLL.DTOs.PurchaseOrder;

public sealed class UpdatePurchaseOrderStatusDto
{
    public PurchaseOrderStatus Status { get; set; }
}
