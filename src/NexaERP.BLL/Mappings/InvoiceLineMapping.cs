using NexaERP.BLL.DTOs.InvoiceLine;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class InvoiceLineMapping
{
    public static InvoiceLine ToEntity(this CreateInvoiceLineDto dto)
    {
        return new InvoiceLine
        {
            Description = dto.Description,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            TaxRate = dto.TaxRate
        };
    }

    public static InvoiceLineDto ToDto(this InvoiceLine invoiceLine)
    {
        return new InvoiceLineDto
        {
            Id = invoiceLine.Id,
            Description = invoiceLine.Description,
            Quantity = invoiceLine.Quantity,
            UnitPrice = invoiceLine.UnitPrice,
            TaxRate = invoiceLine.TaxRate
        };
    }
}
