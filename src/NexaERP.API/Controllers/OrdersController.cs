using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Order;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Enums;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    LinkService linkService)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResult<OrderDto>>> GetOrders(
        [FromQuery] OrderQueryParameters query)
    {
        var orders = orderRepository
            .Filter(
                query.Status,
                query.CustomerId,
                query.From,
                query.To)
            .Select(OrderMapping.ProjectToDto());

        var result = await PaginationResult<OrderDto>.CreateAsync(
            orders,
            query.Page,
            query.PageSize);

        bool includeLinks =
            query.Accept == CustomMediaTypeNames.Application.HateoasJson;

        if (includeLinks)
        {
            // Add HATEOAS links to each order.
            foreach (var order in result.Items)
            {
                order.Links = CreateLinksForOrder(order.Id, order.Status);
            }

            // Add collection links.
            result.Links = CreateLinksForOrders(
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(
        Guid id,
        [FromHeader(Name = "Accept")] string? accept)
    {
        var order = await orderRepository.GetWithLinesAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        var dto = order.ToDto();

        if (accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            dto.Links = CreateLinksForOrder(dto.Id, dto.Status);
        }

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        [FromBody] CreateOrderDto dto,
        [FromServices] IValidator<CreateOrderDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var order = dto.ToEntity();

        await orderRepository.AddAsync(order);
        await unitOfWork.SaveChangesAsync();

        var orderDto = order.ToDto();

        orderDto.Links = CreateLinksForOrder(
            orderDto.Id,
            orderDto.Status);

        return CreatedAtAction(
            nameof(GetById),
            new { id = orderDto.Id },
            orderDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOrderDto dto,
        [FromServices] IValidator<UpdateOrderDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var order = await orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        order.UpdateEntity(dto);

        orderRepository.Update(order);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrderStatusDto dto,
        [FromServices] IValidator<UpdateOrderStatusDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var order = await orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        if (!IsValidStatusTransition(order.Status, dto.Status))
        {
            return BadRequest(
                $"Cannot change order status from {order.Status} to {dto.Status}.");
        }

        order.Status = dto.Status;

        orderRepository.Update(order);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        // Only pending orders can be cancelled/deleted.
        if (order.Status != OrderStatus.Pending)
        {
            return BadRequest(
                "Only orders with Pending status can be deleted.");
        }

        orderRepository.Delete(order);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    private static bool IsValidStatusTransition(
    OrderStatus currentStatus,
    OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending =>
                newStatus == OrderStatus.Confirmed,

            OrderStatus.Confirmed =>
                newStatus == OrderStatus.Shipped,

            OrderStatus.Shipped =>
                newStatus == OrderStatus.Delivered,

            OrderStatus.Delivered =>
                false,

            _ => false
        };
    }

    // Creates HATEOAS links for a single order.
    private List<LinkDto> CreateLinksForOrder(
        Guid id,
        OrderStatus status)
    {
        List<LinkDto> links =
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
        ];

        // Delivered is the final status.
        if (status != OrderStatus.Delivered)
        {
            links.Add(
                linkService.Create(
                    nameof(UpdateStatus),
                    "update-status",
                    HttpMethods.Patch,
                    new { id }));
        }
        // Only pending orders can be deleted.
        if (status == OrderStatus.Pending)
        {
            links.Add(
                linkService.Create(
                    nameof(Delete),
                    "delete",
                    HttpMethods.Delete,
                    new { id }));
        }

        return links;
    }

    // Creates HATEOAS links for the order collection.
    private List<LinkDto> CreateLinksForOrders(
        OrderQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            // Current collection.
            linkService.Create(
                nameof(GetOrders),
                "self",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page,
                    pageSize = parameters.PageSize,
                    status = parameters.Status,
                    customerId = parameters.CustomerId,
                    from = parameters.From,
                    to = parameters.To
                }),

            // Create order.
            linkService.Create(
                nameof(Create),
                "create-order",
                HttpMethods.Post)
        ];

        // Next page.
        if (hasNextPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetOrders),
                    "next-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page + 1,
                        pageSize = parameters.PageSize,
                        status = parameters.Status,
                        customerId = parameters.CustomerId,
                        from = parameters.From,
                        to = parameters.To
                    }));
        }

        // Previous page.
        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetOrders),
                    "previous-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page - 1,
                        pageSize = parameters.PageSize,
                        status = parameters.Status,
                        customerId = parameters.CustomerId,
                        from = parameters.From,
                        to = parameters.To
                    }));
        }

        return links;
    }
}
