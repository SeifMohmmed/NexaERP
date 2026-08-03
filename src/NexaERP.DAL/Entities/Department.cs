namespace NexaERP.DAL.Entities;

public sealed class Department : Entity
{
    public string Name { get; set; }

    // Navigation Property
    public ICollection<Employee> Employees { get; set; }
}
