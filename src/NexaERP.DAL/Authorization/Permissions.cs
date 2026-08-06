namespace NexaERP.DAL.Authorization;

/// <summary>
/// Defines permission names used by the API for permission-based authorization.
/// These constants are referenced in authorization attributes.
/// </summary>
public static class Permissions
{
    // Users
    public const string UsersRead = "users:read";
    public const string UsersUpdateRoles = "users:update-roles";

    // Roles
    public const string RolesRead = "roles:read";
    public const string RolesAssign = "roles:assign";
    public const string RolesRemove = "roles:remove";
    public const string RolesUpdate = "roles:update";

    // Customers
    public const string CustomersRead = "customers:read";
    public const string CustomersCreate = "customers:create";
    public const string CustomersUpdate = "customers:update";
    public const string CustomersDelete = "customers:delete";

    // Suppliers
    public const string SuppliersRead = "suppliers:read";
    public const string SuppliersCreate = "suppliers:create";
    public const string SuppliersUpdate = "suppliers:update";
    public const string SuppliersDelete = "suppliers:delete";

    // Products
    public const string ProductsRead = "products:read";
    public const string ProductsCreate = "products:create";
    public const string ProductsUpdate = "products:update";
    public const string ProductsDelete = "products:delete";
    public const string ProductsAdjustStock = "products:adjust-stock";

    // Categories
    public const string CategoriesRead = "categories:read";
    public const string CategoriesCreate = "categories:create";
    public const string CategoriesUpdate = "categories:update";
    public const string CategoriesDelete = "categories:delete";

    // Departments
    public const string DepartmentsRead = "departments:read";
    public const string DepartmentsCreate = "departments:create";
    public const string DepartmentsUpdate = "departments:update";
    public const string DepartmentsDelete = "departments:delete";

    // Employees
    public const string EmployeesRead = "employees:read";
    public const string EmployeesCreate = "employees:create";
    public const string EmployeesUpdate = "employees:update";
    public const string EmployeesDelete = "employees:delete";

    // Orders
    public const string OrdersRead = "orders:read";
    public const string OrdersCreate = "orders:create";
    public const string OrdersUpdate = "orders:update";
    public const string OrdersUpdateStatus = "orders:update-status";
    public const string OrdersDelete = "orders:delete";

    // Purchase Orders
    public const string PurchaseOrdersRead = "purchase-orders:read";
    public const string PurchaseOrdersCreate = "purchase-orders:create";
    public const string PurchaseOrdersUpdate = "purchase-orders:update";
    public const string PurchaseOrdersUpdateStatus = "purchase-orders:update-status";

    // Invoices
    public const string InvoicesRead = "invoices:read";
    public const string InvoicesCreate = "invoices:create";
    public const string InvoicesUpdate = "invoices:update";
    public const string InvoicesDelete = "invoices:delete";
    public const string InvoicesPay = "invoices:pay";
    public const string InvoicesDownloadPdf = "invoices:download-pdf";
}
