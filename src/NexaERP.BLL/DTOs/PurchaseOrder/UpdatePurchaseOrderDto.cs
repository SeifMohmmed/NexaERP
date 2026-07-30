namespace NexaERP.BLL.DTOs.PurchaseOrder;

public sealed class UpdatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }

    public DateTime ExpectedDelivery { get; set; }
}
