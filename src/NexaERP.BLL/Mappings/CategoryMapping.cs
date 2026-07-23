using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Category;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class CategoryMapping
{
    // Projects Category entities to DTOs.
    public static Expression<Func<Category, CategoryDto>> ProjectToDto()
    {
        return category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}
