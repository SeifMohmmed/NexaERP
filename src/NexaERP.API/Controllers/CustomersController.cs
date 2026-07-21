using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Customer;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Repositories.Abstraction;
namespace NexaERP.API.Controllers;

[Route("customers")]
[ApiController]
public class CustomersController(
    ICustomerRepository customerRepository,
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

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var customer = await customerRepository.GetByIdAsync(id);

        return customer is null ? NotFound() : Ok(customer.ToDto());
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

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            customer.ToDto());
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
}
