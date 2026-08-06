using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Employee;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("employees")]
[ApiController]
public class EmployeesController(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork,
    LinkService linkService)
    : ControllerBase
{

    [HttpGet]
    [HasPermission(Permissions.EmployeesRead)]
    public async Task<ActionResult<PaginationResult<EmployeeDto>>> GetEmployees(
    [FromQuery] EmployeeQueryParameters query)
    {
        var employees = employeeRepository
            .Filter(
                query.DepartmentId,
                query.Search)
            .Select(EmployeeMapping.ProjectToDto());

        var result = await PaginationResult<EmployeeDto>.CreateAsync(
            employees,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
        {
            foreach (var employee in result.Items)
            {
                employee.Links =
                    CreateLinksForEmployee(employee.Id);
            }

            result.Links =
                CreateLinksForEmployees(
                    query,
                    result.HasNextPage,
                    result.HasPreviousPage);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.EmployeesRead)]
    public async Task<ActionResult<EmployeeDto>> GetById(
    Guid id,
    [FromQuery] EmployeeQueryParameters query)
    {
        var employee =
            await employeeRepository.GetWithDepartmentAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        var dto = employee.ToDto();

        if (query.IncludeLinks)
        {
            dto.Links = CreateLinksForEmployee(dto.Id);
        }

        return Ok(dto);
    }


    [HttpPost]
    [HasPermission(Permissions.EmployeesCreate)]
    public async Task<ActionResult<EmployeeDto>> Create(
    [FromBody] CreateEmployeeDto dto,
    [FromServices] IValidator<CreateEmployeeDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var employee = dto.ToEntity();

        await employeeRepository.AddAsync(employee);

        await unitOfWork.SaveChangesAsync();

        employee = await employeeRepository
            .GetWithDepartmentAsync(employee.Id);

        if (employee is null)
        {
            throw new InvalidOperationException(
                "Employee was created but could not be loaded.");
        }

        var employeeDto = employee.ToDto();

        employeeDto.Links =
            CreateLinksForEmployee(employeeDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = employeeDto.Id },
            employeeDto);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permissions.EmployeesUpdate)]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateEmployeeDto dto,
    [FromServices] IValidator<UpdateEmployeeDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        var employee =
            await employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return NotFound();
        }

        employee.UpdateEntity(dto);

        employeeRepository.Update(employee);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.EmployeesDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var employee =
            await employeeRepository.GetByIdAsync(id);

        if (employee is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        employeeRepository.Delete(employee);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }


    private List<LinkDto> CreateLinksForEmployee(Guid id)
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

    private List<LinkDto> CreateLinksForEmployees(
    EmployeeQueryParameters parameters,
    bool hasNextPage,
    bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(
            nameof(GetEmployees),
            "self",
            HttpMethods.Get,
            new
            {
                page = parameters.Page,
                pageSize = parameters.PageSize,
                departmentId = parameters.DepartmentId,
                search = parameters.Search
            }),

        linkService.Create(
            nameof(Create),
            "create-employee",
            HttpMethods.Post)
        ];

        if (hasNextPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetEmployees),
                    "next-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page + 1,
                        pageSize = parameters.PageSize,
                        departmentId = parameters.DepartmentId,
                        search = parameters.Search
                    }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetEmployees),
                    "previous-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page - 1,
                        pageSize = parameters.PageSize,
                        departmentId = parameters.DepartmentId,
                        search = parameters.Search
                    }));
        }

        return links;
    }
}
