using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Employee;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class EmployeeMapping
{
    public static Employee ToEntity(this CreateEmployeeDto dto)
    {
        return new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DepartmentId = dto.DepartmentId,
            Position = dto.Position,
            HireDate = dto.HireDate,
            Salary = dto.Salary
        };
    }

    public static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,

            FirstName = employee.FirstName,
            LastName = employee.LastName,

            Email = employee.Email,
            Phone = employee.Phone,

            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,

            Position = employee.Position,
            HireDate = employee.HireDate,
            Salary = employee.Salary,

            ProfilePhotoUrl = employee.ProfilePhotoUrl
        };
    }

    public static void UpdateEntity(
        this Employee employee,
        UpdateEmployeeDto dto)
    {
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;

        employee.Email = dto.Email;
        employee.Phone = dto.Phone;

        employee.DepartmentId = dto.DepartmentId;

        employee.Position = dto.Position;
        employee.HireDate = dto.HireDate;
        employee.Salary = dto.Salary;
    }

    public static Expression<Func<Employee, EmployeeDto>> ProjectToDto()
    {
        return employee => new EmployeeDto
        {
            Id = employee.Id,

            FirstName = employee.FirstName,
            LastName = employee.LastName,

            Email = employee.Email,
            Phone = employee.Phone,

            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,

            Position = employee.Position,
            HireDate = employee.HireDate,
            Salary = employee.Salary,

            ProfilePhotoUrl = employee.ProfilePhotoUrl
        };
    }
}
