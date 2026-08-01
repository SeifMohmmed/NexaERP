using System.Linq.Expressions;
using NexaERP.BLL.DTOs.PurchaseLine;
using NexaERP.BLL.DTOs.PurchaseOrder;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.Mappings;

public static class PurchaseOrderMapping
{
    public static PurchaseOrder ToEntity(this CreatePurchaseOrderDto dto)
    {
        var purchaseOrder = new PurchaseOrder
        {
            SupplierId = dto.SupplierId,
            OrderDate = DateTime.UtcNow,
            ExpectedDelivery = dto.ExpectedDelivery,
            Status = PurchaseOrderStatus.Pending,

            Lines = dto.Lines
                .Select(line => line.ToEntity())
                .ToList()
        };

        purchaseOrder.TotalAmount = purchaseOrder.Lines
            .Sum(line => line.Quantity * line.UnitCost);

        return purchaseOrder;
    }

    public static PurchaseOrderDto ToDto(this PurchaseOrder purchaseOrder)
    {
        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            SupplierId = purchaseOrder.SupplierId,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDelivery = purchaseOrder.ExpectedDelivery,
            Status = purchaseOrder.Status,
            TotalAmount = purchaseOrder.TotalAmount,

            Lines = purchaseOrder.Lines
                .Select(line => line.ToDto())
                .ToList()
        };
    }

    public static void UpdateEntity(
        this PurchaseOrder purchaseOrder,
        UpdatePurchaseOrderDto dto)
    {
        purchaseOrder.SupplierId = dto.SupplierId;
        purchaseOrder.ExpectedDelivery = dto.ExpectedDelivery;
    }

    public static Expression<Func<PurchaseOrder, PurchaseOrderDto>> ProjectToDto()
    {
        return purchaseOrder => new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            SupplierId = purchaseOrder.SupplierId,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDelivery = purchaseOrder.ExpectedDelivery,
            Status = purchaseOrder.Status,
            TotalAmount = purchaseOrder.TotalAmount,

            Lines = purchaseOrder.Lines
                .Select(line => new PurchaseLineDto
                {
                    Id = line.Id,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost
                })
                .ToList()
        };
    }
}
