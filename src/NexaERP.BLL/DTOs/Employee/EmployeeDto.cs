using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.Employee;

public sealed class EmployeeDto : ILinksResponse
{
    public Guid Id { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; init; }

    public string Phone { get; init; }

    public Guid DepartmentId { get; init; }

    public string DepartmentName { get; init; }

    public string Position { get; init; }

    public DateTime HireDate { get; init; }

    public decimal Salary { get; init; }

    public string? ProfilePhotoUrl { get; init; }

    public List<LinkDto> Links { get; set; }
}
