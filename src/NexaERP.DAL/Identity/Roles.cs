namespace NexaERP.DAL.Identity;

// Defines application roles used throughout the system
public static class Roles
{
    // Full system access.
    public const string Admin = nameof(Admin);

    // Sales operations.
    public const string Sales = nameof(Sales);

    // Purchasing operations.
    public const string Purchasing = nameof(Purchasing);

    // Inventory and warehouse management.
    public const string Warehouse = nameof(Warehouse);

    // Accounting and finance.
    public const string Accountant = nameof(Accountant);

    // Human resources management.
    public const string HR = nameof(HR);
}
