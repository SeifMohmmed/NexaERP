using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.Employee;

public sealed class EmployeeQueryParameters : AcceptHeaderDto
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public Guid? DepartmentId { get; init; }

    // Search term.
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

}
