using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.PurchaseOrder;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;
using NexaERP.DAL.Services;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("purchase-orders")]
[ApiController]
public class PurchaseOrdersController(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    LinkService linkService,
    UserContext userContext)
    : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.PurchaseOrdersRead)]
    public async Task<ActionResult<PaginationResult<PurchaseOrderDto>>> GetPurchaseOrders(
    [FromQuery] PurchaseOrderQueryParameters query)
    {
        Guid? userId = await userContext.GetUserIdAsync();

        if (userId is null)
        {
            return Unauthorized();
        }

        var purchaseOrders = purchaseOrderRepository
            .Query(userId.Value)
            .Select(PurchaseOrderMapping.ProjectToDto());

        var result = await PaginationResult<PurchaseOrderDto>.CreateAsync(
            purchaseOrders,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
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
    [HasPermission(Permissions.PurchaseOrdersRead)]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(
        Guid id,
        [FromQuery] PurchaseOrderQueryParameters query)
    {
        Guid? userId = await userContext.GetUserIdAsync();

        if (userId is null)
        {
            return Unauthorized();
        }

        var purchaseOrder =
            await purchaseOrderRepository.GetWithLinesAsync(
                id,
                userId.Value);

        if (purchaseOrder is null)
        {
            return NotFound();
        }

        var dto = purchaseOrder.ToDto();

        if (query.IncludeLinks)
        {
            dto.Links = CreateLinksForPurchaseOrder(dto.Id);
        }

        return Ok(dto);
    }

    [HttpPost]
    [HasPermission(Permissions.PurchaseOrdersCreate)]
    public async Task<ActionResult<PurchaseOrderDto>> Create(
    [FromBody] CreatePurchaseOrderDto dto,
    [FromServices] IValidator<CreatePurchaseOrderDto> validator)
    {
        Guid? userId = await userContext.GetUserIdAsync();

        if (userId is null)
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(dto);

        var purchaseOrder = dto.ToEntity(userId.Value);

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
    [HasPermission(Permissions.PurchaseOrdersUpdate)]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdatePurchaseOrderDto dto,
    [FromServices] IValidator<UpdatePurchaseOrderDto> validator)
    {
        Guid? userId = await userContext.GetUserIdAsync();

        if (userId is null)
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(dto);

        var purchaseOrder =
            await purchaseOrderRepository.GetByIdAsync(
                id,
                userId.Value);

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
    [HasPermission(Permissions.PurchaseOrdersUpdateStatus)]
    public async Task<IActionResult> UpdateStatus(
    Guid id,
    [FromBody] UpdatePurchaseOrderStatusDto dto,
    [FromServices] IValidator<UpdatePurchaseOrderStatusDto> validator)
    {
        Guid? userId = await userContext.GetUserIdAsync();

        if (userId is null)
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(dto);

        var purchaseOrder =
            await purchaseOrderRepository.GetByIdAsync(
                id,
                userId.Value);

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
