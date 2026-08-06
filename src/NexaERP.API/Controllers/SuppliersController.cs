using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Supplier;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Repositories.Abstraction;
namespace NexaERP.API.Controllers;

[Authorize]
[Route("suppliers")]
[ApiController]
public class SuppliersController(
    ISupplierRepository supplierRepository,
    LinkService linkService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.SuppliersRead)]
    public async Task<ActionResult<PaginationResult<SupplierDto>>> GetSuppliers(
        [FromQuery] SupplierQueryParameters query)
    {
        var suppliers = supplierRepository
            .Query()
            .Select(SupplierMapping.ProjectToDto());

        var result = await PaginationResult<SupplierDto>.CreateAsync(
            suppliers,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
        {
            // Add HATEOAS links to each supplier.
            foreach (var supplier in result.Items)
            {
                supplier.Links = CreateLinksForSupplier(supplier.Id);
            }

            // Add collection links.
            result.Links = CreateLinksForSuppliers(
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }
    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.SuppliersRead)]
    public async Task<ActionResult<SupplierDto>> GetById(
        Guid id,
         [FromQuery] SupplierQueryParameters query)
    {
        var supplier = await supplierRepository.GetByIdAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }

        var supplierDto = supplier.ToDto();

        if (query.IncludeLinks)
        {
            supplierDto.Links = CreateLinksForSupplier(supplierDto.Id);
        }

        return Ok(supplierDto);
    }

    [HttpPost]
    [HasPermission(Permissions.SuppliersCreate)]
    public async Task<ActionResult<SupplierDto>> Create(
       [FromBody] CreateSupplierDto dto,
       [FromServices] IValidator<CreateSupplierDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var supplier = dto.ToEntity();

        await supplierRepository.AddAsync(supplier);
        await unitOfWork.SaveChangesAsync();

        var supplierDto = supplier.ToDto();

        supplierDto.Links = CreateLinksForSupplier(supplierDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = supplierDto.Id },
            supplierDto);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.SuppliersUpdate)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateSupplierDto dto,
        [FromServices] IValidator<UpdateSupplierDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Supplier? supplier = await supplierRepository.GetByIdAsync(id);

        if (supplier is null)
        {
            return NotFound();
        }

        supplier.UpdateEntity(dto);

        supplierRepository.Update(supplier);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.SuppliersDelete)]
    public async Task<ActionResult> Delete(Guid id)
    {
        Supplier? supplier = await supplierRepository.GetByIdAsync(id);

        if (supplier is null)
        {
            return NotFound();
        }

        supplierRepository.Delete(supplier);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    // Creates HATEOAS links for the supplier collection.
    private List<LinkDto> CreateLinksForSuppliers(
        SupplierQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            // Current collection.
            linkService.Create(
            nameof(GetSuppliers),
            "self",
            HttpMethods.Get,
            new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize
            }),

        // Create supplier.
        linkService.Create(
            nameof(Create),
            "create-supplier",
            HttpMethods.Post)
        ];

        // Next page.
        if (hasNextPage)
        {
            links.Add(linkService.Create(
                nameof(GetSuppliers),
                "next-page",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page + 1,
                    pageSize = parameters.PageSize
                }));
        }

        // Previous page.
        if (hasPreviousPage)
        {
            links.Add(linkService.Create(
                nameof(GetSuppliers),
                "previous-page",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page - 1,
                    pageSize = parameters.PageSize
                }));
        }

        return links;
    }

    // Creates HATEOAS links for a single supplier.
    private List<LinkDto> CreateLinksForSupplier(Guid id)
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

        // Delete.
        linkService.Create(
            nameof(Delete),
            "delete",
            HttpMethods.Delete,
            new { id })
        ];
    }
}
