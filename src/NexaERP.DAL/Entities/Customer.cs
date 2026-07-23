using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Entities;

public sealed class Customer : Entity, ISoftDeletable
{
    // Customer name.
    public string Name { get; set; } = default!;

    // Customer email.
    public string Email { get; set; } = default!;

    // Customer phone number.
    public string Phone { get; set; } = default!;

    // Customer address.
    public string Address { get; set; } = default!;

    // Customer city.
    public string City { get; set; } = default!;

    // Customer country.
    public string Country { get; set; } = default!;

    // Customer tax identifier.
    public string? TaxId { get; set; }

    // Indicates whether the customer is soft deleted.
    public bool IsDeleted { get; set; }
}
