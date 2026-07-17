using NexaERP.BLL.DTOs.Customer;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class CustomerMapping
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            TaxId = customer.TaxId,
        };
    }

    public static Customer ToEntity(this CreateCustomerDto dto)
    {
        return new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            TaxId = dto.TaxId,
        };
    }

    public static CustomersCollectionDto ToCollectionDto(this IEnumerable<Customer> customers)
    {
        return new CustomersCollectionDto
        {
            Data = customers
                .Select(c => c.ToDto())
                .ToList()
        };
    }

    public static void UpdateEntity(this Customer customer, UpdateCustomerDto dto)
    {
        customer.Name = dto.Name;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;
        customer.Address = dto.Address;
        customer.City = dto.City;
        customer.Country = dto.Country;
        customer.TaxId = dto.TaxId;
    }
}
