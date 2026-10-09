namespace NexaERP.BLL.DTOs.InventoryReport;

public sealed class InventoryReportQueryParameters
{
    public Guid? CategoryId { get; set; }

    public bool? LowStock { get; set; }
}
