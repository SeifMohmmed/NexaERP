using Microsoft.AspNetCore.Mvc;

namespace NexaERP.BLL.DTOs.Employee;

public sealed class EmployeeQueryParameters
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public Guid? DepartmentId { get; init; }

    // Search term.
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    // Requested response media type.
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }
}
