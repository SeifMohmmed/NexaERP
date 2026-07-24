using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Customer;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Repositories.Abstraction;
namespace NexaERP.API.Controllers;

[Route("customers")]
[ApiController]
public class CustomersController(
    ICustomerRepository customerRepository,
    LinkService linkService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginationResult<CustomerDto>>> GetCustomers(
        [FromQuery] CustomerQueryParameters query)
    {
        IQueryable<CustomerDto> customersQuery = customerRepository
            .Search(query.Search)
            .Select(CustomerMapping.ProjectToDto());

        var result = await PaginationResult<CustomerDto>.CreateAsync(
            customersQuery,
            query.Page,
            query.PageSize);

        foreach (var customer in result.Items)
        {
            customer.Links = CreateLinksForCustomer(customer.Id);
        }

        result.Links = CreateLinksForCustomers(
            query,
            result.HasNextPage,
            result.HasPreviousPage);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        var dto = customer.ToDto();
        dto.Links = CreateLinksForCustomer(dto.Id);

        return Ok(dto);

    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(
       [FromBody] CreateCustomerDto dto,
       [FromServices] IValidator<CreateCustomerDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var customer = dto.ToEntity();

        await customerRepository.AddAsync(customer);
        await unitOfWork.SaveChangesAsync();

        var customerDto = customer.ToDto();
        customerDto.Links = CreateLinksForCustomer(customerDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customerDto.Id },
            customerDto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(
        Guid id,
        [FromBody] UpdateCustomerDto dto,
        [FromServices] IValidator<UpdateCustomerDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var customer = await customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        customer.UpdateEntity(dto);

        customerRepository.Update(customer);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var customer = await customerRepository.GetByIdAsync(id);

        if (customer is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        customerRepository.Delete(customer);
        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForCustomers(
    CustomerQueryParameters parameters,
    bool hasNextPage,
    bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(
            nameof(GetCustomers),
            "self",
            HttpMethods.Get,
            new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                search = parameters.Search
            }),

        linkService.Create(
            nameof(Create),
            "create-customer",
            HttpMethods.Post)
        ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(
                nameof(GetCustomers),
                "next-page",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page + 1,
                    pageSize = parameters.PageSize,
                    search = parameters.Search
                }));
        }

        if (hasPreviousPage)
        {
            links.Add(linkService.Create(
                nameof(GetCustomers),
                "previous-page",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page - 1,
                    pageSize = parameters.PageSize,
                    search = parameters.Search
                }));
        }

        return links;
    }

    private List<LinkDto> CreateLinksForCustomer(Guid id)
    {
        return
        [
            linkService.Create(
            nameof(GetById),
            "self",
            HttpMethods.Get,
            new { id }),

        linkService.Create(
            nameof(Update),
            "update",
            HttpMethods.Put,
            new { id }),

        linkService.Create(
            nameof(Delete),
            "delete",
            HttpMethods.Delete,
            new { id })
        ];
    }
}
