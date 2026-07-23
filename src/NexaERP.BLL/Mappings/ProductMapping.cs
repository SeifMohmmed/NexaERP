using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Product;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class ProductMapping
{
    // Projects Product entities to DTOs.
    public static Expression<Func<Product, ProductDto>> ProjectToDto()
    {
        return product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            UnitPrice = product.UnitPrice,
            CostPrice = product.CostPrice,
            StockQuantity = product.StockQuantity,
            ReorderLevel = product.ReorderLevel
        };
    }

    // Maps a Product entity to a DTO.
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            UnitPrice = product.UnitPrice,
            CostPrice = product.CostPrice,
            StockQuantity = product.StockQuantity,
            ReorderLevel = product.ReorderLevel
        };
    }

    // Maps a create DTO to a Product entity.
    public static Product ToEntity(this CreateProductDto dto)
    {
        return new Product
        {
            Id = Guid.CreateVersion7(),
            Name = dto.Name,
            SKU = dto.SKU,
            CategoryId = dto.CategoryId,
            UnitPrice = dto.UnitPrice,
            CostPrice = dto.CostPrice,
            StockQuantity = dto.StockQuantity,
            ReorderLevel = dto.ReorderLevel
        };
    }

    // Updates an existing Product entity.
    public static void UpdateEntity(
        this Product product,
        UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.SKU = dto.SKU;
        product.CategoryId = dto.CategoryId;
        product.UnitPrice = dto.UnitPrice;
        product.CostPrice = dto.CostPrice;
        product.StockQuantity = dto.StockQuantity;
        product.ReorderLevel = dto.ReorderLevel;
    }
}
