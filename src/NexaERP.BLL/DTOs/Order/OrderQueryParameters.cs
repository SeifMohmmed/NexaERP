using Microsoft.AspNetCore.Mvc;
using NexaERP.DAL.Enums;

namespace NexaERP.BLL.DTOs.Order;

public sealed class OrderQueryParameters
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public OrderStatus? Status { get; init; }

    public Guid? CustomerId { get; init; }

    public DateOnly? From { get; init; }

    public DateOnly? To { get; init; }

    // Requested response media type.
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }
}
