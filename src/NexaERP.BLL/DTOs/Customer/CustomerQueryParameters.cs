using Microsoft.AspNetCore.Mvc;

namespace NexaERP.BLL.DTOs.Customer;

/// <summary>
/// Query parameters for customer filtering and pagination.
/// </summary>
public sealed record CustomerQueryParameters
{
    // Search term.
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    // Requested page number.
    public int Page { get; init; } = 1;

    // Number of items per page.
    public int PageSize { get; init; } = 10;
}
