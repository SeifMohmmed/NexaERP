namespace NexaERP.BLL.DTOs.Customer;

public sealed record CustomersCollectionDto
{
    public List<CustomerDto> Data { get; init; } = [];
}

public sealed record CustomerDto
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string Email { get; init; }

    public string Phone { get; init; }

    public string Address { get; init; }

    public string City { get; init; }

    public string Country { get; init; }

    public string? TaxId { get; init; }
}
