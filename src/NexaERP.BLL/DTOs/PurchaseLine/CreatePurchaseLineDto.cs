namespace NexaERP.BLL.DTOs.PurchaseLine;

public sealed class CreatePurchaseLineDto
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }
}
