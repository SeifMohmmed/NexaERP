using System.Linq.Expressions;
using NexaERP.BLL.DTOs.Invoice;
using NexaERP.BLL.DTOs.InvoiceLine;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.Mappings;

public static class InvoiceMapping
{
    public static Invoice ToEntity(this CreateInvoiceDto dto)
    {
        var invoice = new Invoice
        {
            CustomerId = dto.CustomerId,
            InvoiceDate = dto.InvoiceDate,
            DueDate = dto.DueDate,
            Status = InvoiceStatus.Draft,

            Lines = dto.Lines
                .Select(line => line.ToEntity())
                .ToList()
        };

        invoice.TotalAmount = invoice.Lines.Sum(line =>
        {
            var subTotal = line.Quantity * line.UnitPrice;
            var tax = subTotal * (line.TaxRate / 100m);

            return subTotal + tax;
        });

        return invoice;
    }

    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,

            CustomerName = invoice.Customer.Name,
            CustomerEmail = invoice.Customer.Email,
            CustomerPhone = invoice.Customer.Phone,
            CustomerAddress = invoice.Customer.Address,

            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            PaidAt = invoice.PaidAt,
            PaymentMethod = invoice.PaymentMethod,

            Lines = invoice.Lines
                .Select(line => line.ToDto())
                .ToList()
        };
    }

    public static void UpdateEntity(
        this Invoice invoice,
        UpdateInvoiceDto dto)
    {
        invoice.CustomerId = dto.CustomerId;
        invoice.InvoiceDate = dto.InvoiceDate;
        invoice.DueDate = dto.DueDate;
    }

    public static Expression<Func<Invoice, InvoiceDto>> ProjectToDto()
    {
        return invoice => new InvoiceDto
        {
            Id = invoice.Id,
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,

            CustomerName = invoice.Customer.Name,
            CustomerEmail = invoice.Customer.Email,
            CustomerPhone = invoice.Customer.Phone,
            CustomerAddress = invoice.Customer.Address,

            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            PaidAt = invoice.PaidAt,
            PaymentMethod = invoice.PaymentMethod,

            Lines = invoice.Lines
                .Select(line => new InvoiceLineDto
                {
                    Id = line.Id,
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TaxRate = line.TaxRate
                })
                .ToList()
        };
    }
}
