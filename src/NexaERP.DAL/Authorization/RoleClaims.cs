using NexaERP.DAL.Identity;

namespace NexaERP.DAL.Authorization;

internal static class RoleClaims
{
    // Maps each role to its assigned permissions.
    public static IReadOnlyDictionary<string, IReadOnlyCollection<string>> Map { get; } =
        new Dictionary<string, IReadOnlyCollection<string>>
        {
            [Roles.Admin] =
            [
                Permissions.UsersRead,
                Permissions.UsersUpdateRoles,

                Permissions.RolesRead,
                Permissions.RolesAssign,
                Permissions.RolesRemove,
                Permissions.RolesUpdate,

                Permissions.CustomersRead,
                Permissions.CustomersCreate,
                Permissions.CustomersUpdate,
                Permissions.CustomersDelete,

                Permissions.SuppliersRead,
                Permissions.SuppliersCreate,
                Permissions.SuppliersUpdate,
                Permissions.SuppliersDelete,

                Permissions.ProductsRead,
                Permissions.ProductsCreate,
                Permissions.ProductsUpdate,
                Permissions.ProductsDelete,
                Permissions.ProductsAdjustStock,

                Permissions.CategoriesRead,
                Permissions.CategoriesCreate,
                Permissions.CategoriesUpdate,
                Permissions.CategoriesDelete,

                Permissions.DepartmentsRead,
                Permissions.DepartmentsCreate,
                Permissions.DepartmentsUpdate,
                Permissions.DepartmentsDelete,

                Permissions.EmployeesRead,
                Permissions.EmployeesCreate,
                Permissions.EmployeesUpdate,
                Permissions.EmployeesDelete,

                Permissions.OrdersRead,
                Permissions.OrdersCreate,
                Permissions.OrdersUpdate,
                Permissions.OrdersUpdateStatus,
                Permissions.OrdersDelete,

                Permissions.PurchaseOrdersRead,
                Permissions.PurchaseOrdersCreate,
                Permissions.PurchaseOrdersUpdate,
                Permissions.PurchaseOrdersUpdateStatus,

                Permissions.InvoicesRead,
                Permissions.InvoicesCreate,
                Permissions.InvoicesUpdate,
                Permissions.InvoicesDelete,
                Permissions.InvoicesPay,
                Permissions.InvoicesDownloadPdf
            ],

            [Roles.Sales] =
            [
                Permissions.CustomersRead,
                Permissions.CustomersCreate,
                Permissions.CustomersUpdate,

                Permissions.OrdersRead,
                Permissions.OrdersCreate,
                Permissions.OrdersUpdate,

                Permissions.InvoicesRead,
                Permissions.InvoicesCreate
            ],

            [Roles.Purchasing] =
            [
                Permissions.SuppliersRead,
                Permissions.SuppliersCreate,
                Permissions.SuppliersUpdate,

                Permissions.ProductsRead,
                Permissions.ProductsCreate,
                Permissions.ProductsUpdate,

                Permissions.CategoriesRead,

                Permissions.PurchaseOrdersRead,
                Permissions.PurchaseOrdersCreate,
                Permissions.PurchaseOrdersUpdate
            ],

            [Roles.Warehouse] =
            [
                Permissions.ProductsRead,
                Permissions.ProductsAdjustStock,

                Permissions.CategoriesRead,

                Permissions.PurchaseOrdersRead,
                Permissions.PurchaseOrdersUpdateStatus
            ],

            [Roles.Accountant] =
            [
                Permissions.CustomersRead,

                Permissions.SuppliersRead,

                Permissions.InvoicesRead,
                Permissions.InvoicesPay,

                Permissions.PurchaseOrdersRead,
                Permissions.PurchaseOrdersUpdateStatus
            ],

            [Roles.HR] =
            [
                Permissions.EmployeesRead,
                Permissions.EmployeesCreate,
                Permissions.EmployeesUpdate,
                Permissions.EmployeesDelete,

                Permissions.DepartmentsRead,
                Permissions.DepartmentsCreate
            ]
        };
}
