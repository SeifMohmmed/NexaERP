namespace NexaERP.BLL.DTOs.Common;

/// <summary>
/// Represents a collection response.
/// </summary>
public interface ICollectionResponse<T>
{
    // Collection of returned items.
    List<T> Items { get; init; }
}
