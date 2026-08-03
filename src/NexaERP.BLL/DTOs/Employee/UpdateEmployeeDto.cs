namespace NexaERP.BLL.DTOs.Employee;

public sealed class UpdateEmployeeDto
{
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string Email { get; init; }

    public string Phone { get; init; }

    public Guid DepartmentId { get; init; }

    public string Position { get; init; }

    public DateTime HireDate { get; init; }

    public decimal Salary { get; init; }
}
