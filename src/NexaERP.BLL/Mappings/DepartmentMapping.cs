using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Department;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class DepartmentMapping
{
    public static DepartmentDto ToDto(
        this Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name
        };
    }

    public static Expression<Func<Department, DepartmentDto>>
        ProjectToDto()
    {
        return department => new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name
        };
    }
    public static Department ToEntity(
    this CreateDepartmentDto dto)
    {
        return new Department
        {
            Name = dto.Name
        };
    }
}
