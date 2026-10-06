using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Common;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Project;

public sealed class ProjectQueryParameters : AcceptHeaderDto
{
    public ProjectStatus? Status { get; set; }

    public Guid? OwnerId { get; set; }
    
    // Search term.
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
