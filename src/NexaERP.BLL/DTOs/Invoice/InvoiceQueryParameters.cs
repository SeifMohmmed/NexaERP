using NexaERP.BLL.DTOs.Common;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Invoice;

public sealed class InvoiceQueryParameters : AcceptHeaderDto
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public InvoiceStatus? Status { get; init; }

    public Guid? CustomerId { get; init; }

    public DateTime? From { get; init; }

    public DateTime? To { get; init; }
}
