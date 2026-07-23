namespace NexaERP.BLL.DTOs.Category;

/// <summary>
/// Represents category data.
/// </summary>
public sealed class CategoryDto
{
    // Category identifier.
    public Guid Id { get; init; }

    // Category name.
    public string Name { get; init; }
}
