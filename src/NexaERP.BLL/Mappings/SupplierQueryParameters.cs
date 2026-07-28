using Microsoft.AspNetCore.Mvc;

namespace NexaERP.BLL.Mappings;

public sealed class SupplierQueryParameters
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    // Requested response media type.
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }
}
