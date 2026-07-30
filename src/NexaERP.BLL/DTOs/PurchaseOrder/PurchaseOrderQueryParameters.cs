using Microsoft.AspNetCore.Mvc;

namespace NexaERP.BLL.DTOs.PurchaseOrder;

public sealed class PurchaseOrderQueryParameters
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    // Requested response media type.
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }
}
