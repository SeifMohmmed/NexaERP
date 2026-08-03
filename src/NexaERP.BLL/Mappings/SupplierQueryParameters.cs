using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.Mappings;

public sealed class SupplierQueryParameters : AcceptHeaderDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
