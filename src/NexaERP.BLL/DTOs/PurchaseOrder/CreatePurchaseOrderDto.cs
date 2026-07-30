using NexaERP.BLL.DTOs.PurchaseLine;

namespace NexaERP.BLL.DTOs.PurchaseOrder;

public sealed class CreatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }

    public DateTime ExpectedDelivery { get; set; }

    public List<CreatePurchaseLineDto> Lines { get; set; }
}
