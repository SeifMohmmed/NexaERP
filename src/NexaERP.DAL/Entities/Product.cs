using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Entities;

public class Product : Entity, ISoftDeletable
{
    // Product name.
    public string Name { get; set; }

    // Stock Keeping Unit (SKU).
    public string SKU { get; set; }

    // Selling price.
    public decimal UnitPrice { get; set; }

    // Purchase cost.
    public decimal CostPrice { get; set; }

    // Available stock.
    public int StockQuantity { get; set; }

    // Minimum stock level.
    public int ReorderLevel { get; set; }

    // Indicates whether the product is soft deleted.
    public bool IsDeleted { get; set; }

    // Related category ID.
    public Guid CategoryId { get; set; }

    // Related category.
    public Category Category { get; set; }

    // Adjusts the stock quantity.
    public void AdjustStock(int quantityChange)
    {
        // Prevent negative stock.
        if (StockQuantity + quantityChange < 0)
        {
            throw new InvalidOperationException(
                "Stock quantity cannot be negative.");
        }

        StockQuantity += quantityChange;
    }
}
