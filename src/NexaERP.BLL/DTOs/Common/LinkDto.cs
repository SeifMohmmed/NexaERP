namespace NexaERP.BLL.DTOs.Common;

/// <summary>
/// Represents a HATEOAS link.
/// </summary>
public sealed class LinkDto
{
    // Resource URL.
    public required string Href { get; init; }

    // Link relation.
    public required string Rel { get; init; }

    // HTTP method.
    public required string Method { get; init; }
}
