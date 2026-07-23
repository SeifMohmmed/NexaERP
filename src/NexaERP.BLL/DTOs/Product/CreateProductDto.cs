namespace NexaERP.BLL.DTOs.Product;

/// <summary>
/// Represents product creation data.
/// </summary>
public sealed class CreateProductDto
{
    // Product name.
    public string Name { get; init; }

    // Stock keeping unit.
    public string SKU { get; init; }

    // Product category.
    public Guid CategoryId { get; init; }

    // Selling price.
    public decimal UnitPrice { get; init; }

    // Purchase cost.
    public decimal CostPrice { get; init; }

    // Initial stock quantity.
    public int StockQuantity { get; init; }

    // Minimum stock level.
    public int ReorderLevel { get; init; }
}
