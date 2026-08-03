using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.Product;

/// <summary>
/// Query parameters for product filtering and pagination.
/// </summary>
public sealed class ProductQueryParameters : AcceptHeaderDto
{
    // Requested page number.
    public int Page { get; init; } = 1;

    // Number of items per page.
    public int PageSize { get; init; } = 10;

    // Filter by category.
    public Guid? CategoryId { get; init; }

    // Filter low-stock products.
    public bool? LowStock { get; init; }
}
