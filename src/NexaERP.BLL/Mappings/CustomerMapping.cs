using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Customer;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class CustomerMapping
{
    // Maps a Customer entity to a DTO.
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

    // Maps a create DTO to a Customer entity.
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

    // Projects Customer entities directly to DTOs.
    public static Expression<Func<Customer, CustomerDto>> ProjectToDto()
    {
        return customer => new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            TaxId = customer.TaxId
        };
    }

    // Updates an existing Customer entity.
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
