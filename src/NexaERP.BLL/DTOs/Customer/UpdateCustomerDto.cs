namespace NexaERP.BLL.DTOs.Customer;

public sealed record UpdateCustomerDto
{
    public string Name { get; init; }

    public string Email { get; init; }

    public string Phone { get; init; }

    public string Address { get; init; }

    public string City { get; init; }

    public string Country { get; init; }

    public string? TaxId { get; init; }
}
