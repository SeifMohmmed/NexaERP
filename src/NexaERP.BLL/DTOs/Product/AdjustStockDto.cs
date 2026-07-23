namespace NexaERP.BLL.DTOs.Product;

/// <summary>
/// Represents a stock adjustment request.
/// </summary>
public sealed class AdjustStockDto
{
    // Quantity to add or remove.
    public int QuantityChange { get; init; }

    // Reason for the adjustment.
    public string Reason { get; init; }
}
