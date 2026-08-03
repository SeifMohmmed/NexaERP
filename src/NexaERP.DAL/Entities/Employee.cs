using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Entities;

public sealed class Employee : Entity, ISoftDeletable
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public Guid DepartmentId { get; set; }

    public Department Department { get; set; }

    public string Position { get; set; }

    public DateTime HireDate { get; set; }

    public decimal Salary { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public bool IsDeleted { get; set; }
}
