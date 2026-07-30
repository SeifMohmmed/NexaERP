namespace NexaERP.BLL.DTOs.PurchaseLine;

public sealed class PurchaseLineDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }
}
