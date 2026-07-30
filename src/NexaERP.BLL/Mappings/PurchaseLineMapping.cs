using NexaERP.BLL.DTOs.PurchaseLine;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class PurchaseLineMapping
{
    public static PurchaseLine ToEntity(this CreatePurchaseLineDto dto)
    {
        return new PurchaseLine
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost
        };
    }

    public static PurchaseLineDto ToDto(this PurchaseLine purchaseLine)
    {
        return new PurchaseLineDto
        {
            Id = purchaseLine.Id,
            ProductId = purchaseLine.ProductId,
            Quantity = purchaseLine.Quantity,
            UnitCost = purchaseLine.UnitCost
        };
    }
}
