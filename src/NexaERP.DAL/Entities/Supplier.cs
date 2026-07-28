namespace NexaERP.DAL.Entities;

public sealed class Supplier : Entity
{
    public string CompanyName { get; set; }

    public string ContactName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string PaymentTerms { get; set; }
}
