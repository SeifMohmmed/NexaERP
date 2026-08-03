using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.Product;

/// <summary>
/// Represents product data.
/// </summary>
public sealed class ProductDto : ILinksResponse
{
    // Product identifier.
    public Guid Id { get; init; }

    // Product name.
    public string Name { get; init; }

    // Stock keeping unit.
    public string SKU { get; init; }

    // Product category.
    public Guid CategoryId { get; init; }

    // Category name.
    public string CategoryName { get; init; }

    // Selling price.
    public decimal UnitPrice { get; init; }

    // Purchase cost.
    public decimal CostPrice { get; init; }

    // Available stock.
    public int StockQuantity { get; init; }

    // Minimum stock level.
    public int ReorderLevel { get; init; }

    // Resource links.
    public List<LinkDto> Links { get; set; }
}
