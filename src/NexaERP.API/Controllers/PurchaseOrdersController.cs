using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.PurchaseOrder;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Route("purchase-orders")]
[ApiController]
public class PurchaseOrdersController(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    LinkService linkService)
    : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<PaginationResult<PurchaseOrderDto>>> GetPurchaseOrders(
    [FromQuery] PurchaseOrderQueryParameters query)
    {
        var purchaseOrders = purchaseOrderRepository
            .Query()
            .Select(PurchaseOrderMapping.ProjectToDto());

        var result = await PaginationResult<PurchaseOrderDto>.CreateAsync(
            purchaseOrders,
            query.Page,
            query.PageSize);

        bool includeLinks =
            query.Accept == CustomMediaTypeNames.Application.HateoasJson;

        if (includeLinks)
        {
            // Add HATEOAS links to each purchase order.
            foreach (var purchaseOrder in result.Items)
            {
                purchaseOrder.Links =
                    CreateLinksForPurchaseOrder(purchaseOrder.Id);
            }

            // Add collection links.
            result.Links = CreateLinksForPurchaseOrders(
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(
        Guid id,
        [FromHeader(Name = "Accept")] string? accept)
    {
        var purchaseOrder =
            await purchaseOrderRepository.GetWithLinesAsync(id);

        if (purchaseOrder is null)
        {
            return NotFound();
        }

        var dto = purchaseOrder.ToDto();

        if (accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            dto.Links = CreateLinksForPurchaseOrder(dto.Id);
        }

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> Create(
    [FromBody] CreatePurchaseOrderDto dto,
    [FromServices] IValidator<CreatePurchaseOrderDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var purchaseOrder = dto.ToEntity();

        await purchaseOrderRepository.AddAsync(purchaseOrder);

        await unitOfWork.SaveChangesAsync();

        var purchaseOrderDto = purchaseOrder.ToDto();

        purchaseOrderDto.Links =
            CreateLinksForPurchaseOrder(purchaseOrderDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = purchaseOrderDto.Id },
            purchaseOrderDto);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdatePurchaseOrderDto dto,
    [FromServices] IValidator<UpdatePurchaseOrderDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var purchaseOrder =
            await purchaseOrderRepository.GetByIdAsync(id);

        if (purchaseOrder is null)
        {
            return NotFound();
        }

        purchaseOrder.UpdateEntity(dto);

        purchaseOrderRepository.Update(purchaseOrder);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
    Guid id,
    [FromBody] UpdatePurchaseOrderStatusDto dto,
    [FromServices] IValidator<UpdatePurchaseOrderStatusDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var purchaseOrder =
            await purchaseOrderRepository.GetByIdAsync(id);

        if (purchaseOrder is null)
        {
            return NotFound();
        }

        purchaseOrder.Status = dto.Status;

        purchaseOrderRepository.Update(purchaseOrder);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    // Creates HATEOAS links for a single purchase order.
    private List<LinkDto> CreateLinksForPurchaseOrder(Guid id)
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

        // Update status.
        linkService.Create(
            nameof(UpdateStatus),
            "update-status",
            HttpMethods.Patch,
            new { id })
        ];
    }

    // Creates HATEOAS links for the purchase order collection.
    private List<LinkDto> CreateLinksForPurchaseOrders(
        PurchaseOrderQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            // Current collection.
            linkService.Create(
            nameof(GetPurchaseOrders),
            "self",
            HttpMethods.Get,
            new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize
            }),

        // Create purchase order.
        linkService.Create(
            nameof(Create),
            "create-purchase-order",
            HttpMethods.Post)
        ];

        // Next page.
        if (hasNextPage)
        {
            links.Add(linkService.Create(
                nameof(GetPurchaseOrders),
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
                nameof(GetPurchaseOrders),
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
}
