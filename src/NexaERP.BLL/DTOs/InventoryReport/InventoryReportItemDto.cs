namespace NexaERP.BLL.DTOs.InventoryReport;

public sealed class InventoryReportItemDto
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = default!;

    public string SKU { get; set; } = default!;

    public Guid CategoryId { get; set; }

    public int StockQuantity { get; set; }

    public int ReorderLevel { get; set; }

    public bool IsLowStock { get; set; }
}
