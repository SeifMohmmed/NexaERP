namespace NexaERP.BLL.DTOs.Common;

/// <summary>
/// Represents a response containing HATEOAS links.
/// </summary>
public interface ILinksResponse
{
    // Resource links.
    List<LinkDto> Links { get; set; }
}
