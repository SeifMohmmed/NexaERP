using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NexaERP.BLL.DTOs.Department;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("departments")]
[ApiController]
public class DepartmentsController(
    IDepartmentRepository repository)
    : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.DepartmentsRead)]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
    {
        var departments = await repository
            .Query()
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(DepartmentMapping.ProjectToDto())
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.DepartmentsRead)]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id)
    {
        var department = await repository.GetByIdAsync(id);

        if (department is null)
        {
            return NotFound();
        }

        return Ok(department.ToDto());
    }

    [HttpPost]
    [HasPermission(Permissions.DepartmentsCreate)]
    public async Task<ActionResult<DepartmentDto>> Create(
    [FromBody] CreateDepartmentDto dto,
    [FromServices] IValidator<CreateDepartmentDto> validator,
    [FromServices] IUnitOfWork unitOfWork)
    {
        await validator.ValidateAndThrowAsync(dto);

        var exists = await repository.ExistsAsync(
            d => d.Name == dto.Name);

        if (exists)
        {
            return Conflict(
                $"Department '{dto.Name}' already exists.");
        }

        var department = dto.ToEntity();

        await repository.AddAsync(department);

        await unitOfWork.SaveChangesAsync();

        var departmentDto = department.ToDto();

        return CreatedAtAction(
            nameof(GetById),
            new { id = departmentDto.Id },
            departmentDto);
    }
}
