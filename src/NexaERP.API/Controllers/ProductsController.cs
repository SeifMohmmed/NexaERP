using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Product;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Route("products")]
[ApiController]
public class ProductsController(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResult<ProductDto>>> GetProducts(
    [FromQuery] ProductQueryParameters query)
    {
        var products = productRepository.Filter(query.CategoryId, query.LowStock)
            .Select(ProductMapping.ProjectToDto());

        var result = await PaginationResult<ProductDto>.CreateAsync(
            products,
            query.Page,
            query.PageSize);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);

        return product is null ? NotFound() : Ok(product.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
    [FromBody] CreateProductDto dto,
    [FromServices] IValidator<CreateProductDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Product product = dto.ToEntity();

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        product = await productRepository.GetByIdAsync(product.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product!.Id },
            product.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(
    Guid id,
    [FromBody] UpdateProductDto dto,
    [FromServices] IValidator<UpdateProductDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Product? product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        product.UpdateEntity(dto);

        productRepository.Update(product);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/stock")]
    public async Task<ActionResult> AdjustStock(
    Guid id,
    [FromBody] AdjustStockDto dto,
    [FromServices] IValidator<AdjustStockDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Product? product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        product.AdjustStock(dto.QuantityChange);

        productRepository.Update(product);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        Product? product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        productRepository.Delete(product);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}
