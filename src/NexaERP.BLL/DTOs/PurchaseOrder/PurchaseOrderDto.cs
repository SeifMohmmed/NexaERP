using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.PurchaseLine;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.PurchaseOrder;

public sealed class PurchaseOrderDto
{
    public Guid Id { get; init; }

    public Guid SupplierId { get; init; }

    public DateTime OrderDate { get; init; }

    public DateTime ExpectedDelivery { get; init; }

    public PurchaseOrderStatus Status { get; init; }

    public decimal TotalAmount { get; init; }

    public List<PurchaseLineDto> Lines { get; init; }

    public List<LinkDto> Links { get; set; }
}
