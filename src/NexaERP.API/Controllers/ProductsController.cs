using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Product;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Authorize]
[Route("products")]
[ApiController]
public class ProductsController(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    LinkService linkService) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.ProductsRead)]
    public async Task<ActionResult<PaginationResult<ProductDto>>> GetProducts(
    [FromQuery] ProductQueryParameters query)
    {
        var products = productRepository.Filter(query.CategoryId, query.LowStock)
            .Select(ProductMapping.ProjectToDto());

        var result = await PaginationResult<ProductDto>.CreateAsync(
            products,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
        {
            // Add HATEOAS links to each product.
            foreach (var product in result.Items)
            {
                product.Links = CreateLinksForProduct(product.Id);
            }

            // Add collection links.
            result.Links = CreateLinksForProducts(
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.ProductsRead)]
    public async Task<ActionResult<ProductDto>> GetById(
        Guid id,
        [FromQuery] ProductQueryParameters query)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        var dto = product.ToDto();

        if (query.IncludeLinks)
        {
            dto.Links = CreateLinksForProduct(dto.Id);
        }

        return Ok(dto);
    }

    [HttpPost]
    [HasPermission(Permissions.ProductsCreate)]
    public async Task<ActionResult<ProductDto>> Create(
    [FromBody] CreateProductDto dto,
    [FromServices] IValidator<CreateProductDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Product product = dto.ToEntity();

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        product = await productRepository.GetByIdAsync(product.Id);

        var productDto = product!.ToDto();
        productDto.Links = CreateLinksForProduct(productDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = productDto!.Id },
            productDto);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.ProductsUpdate)]
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
    [HasPermission(Permissions.ProductsAdjustStock)]
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
    [HasPermission(Permissions.ProductsDelete)]
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

    // Creates HATEOAS links for the product collection.
    private List<LinkDto> CreateLinksForProducts(
        ProductQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            // Current collection.
            linkService.Create(
                nameof(GetProducts),
                "self",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page,
                    pageSize = parameters.PageSize,
                    categoryId = parameters.CategoryId,
                    lowStock = parameters.LowStock
                }),

            // Create product.
            linkService.Create(
                nameof(Create),
                "create-product",
                HttpMethods.Post)
        ];

        // Next page.
        if (hasNextPage)
        {
            links.Add(linkService.Create(
                nameof(GetProducts),
                "next-page",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page + 1,
                    pageSize = parameters.PageSize,
                    categoryId = parameters.CategoryId,
                    lowStock = parameters.LowStock
                }));
        }

        // Previous page.
        if (hasPreviousPage)
        {
            links.Add(linkService.Create(
                nameof(GetProducts),
                "previous-page",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page - 1,
                    pageSize = parameters.PageSize,
                    categoryId = parameters.CategoryId,
                    lowStock = parameters.LowStock
                }));
        }

        return links;
    }

    // Creates HATEOAS links for a single product.
    private List<LinkDto> CreateLinksForProduct(Guid id)
    {
        return
        [
            // Self.
            linkService.Create(
                nameof(GetById),
                "self",
                HttpMethods.Get,
                new { id }),

            // Update.
            linkService.Create(
                nameof(Update),
                "update",
                HttpMethods.Put,
                new { id }),

            // Adjust stock.
            linkService.Create(
                nameof(AdjustStock),
                "adjust-stock",
                HttpMethods.Patch,
                new { id }),

            // Delete.
            linkService.Create(
                nameof(Delete),
                "delete",
                HttpMethods.Delete,
                new { id })
        ];
    }
}
