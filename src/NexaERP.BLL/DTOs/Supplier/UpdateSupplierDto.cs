namespace NexaERP.BLL.DTOs.Supplier;

public sealed class UpdateSupplierDto
{
    public string CompanyName { get; init; }

    public string ContactName { get; init; }

    public string Email { get; init; }

    public string Phone { get; init; }

    public string PaymentTerms { get; init; }
}
