using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.PurchaseOrder;

public sealed class PurchaseOrderQueryParameters : AcceptHeaderDto
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
