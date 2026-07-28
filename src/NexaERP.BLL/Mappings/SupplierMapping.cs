using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Supplier;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class SupplierMapping
{
    public static Supplier ToEntity(this CreateSupplierDto dto)
    {
        return new Supplier
        {
            CompanyName = dto.CompanyName,
            ContactName = dto.ContactName,
            Email = dto.Email,
            Phone = dto.Phone,
            PaymentTerms = dto.PaymentTerms
        };
    }

    public static SupplierDto ToDto(this Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            CompanyName = supplier.CompanyName,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone,
            PaymentTerms = supplier.PaymentTerms
        };
    }
    public static void UpdateEntity(
    this Supplier supplier,
    UpdateSupplierDto dto)
    {
        supplier.CompanyName = dto.CompanyName;
        supplier.ContactName = dto.ContactName;
        supplier.Email = dto.Email;
        supplier.Phone = dto.Phone;
        supplier.PaymentTerms = dto.PaymentTerms;
    }

    public static Expression<Func<Supplier, SupplierDto>> ProjectToDto()
    {
        return supplier => new SupplierDto
        {
            Id = supplier.Id,
            CompanyName = supplier.CompanyName,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone,
            PaymentTerms = supplier.PaymentTerms
        };
    }
}
