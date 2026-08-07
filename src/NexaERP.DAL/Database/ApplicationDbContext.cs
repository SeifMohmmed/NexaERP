using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexaERP.DAL.Context;
using NexaERP.DAL.Database.Configurations.Application;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
     IHttpContextAccessor httpContextAccessor) : DbContext(options), IUnitOfWork
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseLine> PurchaseLines { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceLine> InvoiceLines { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set the default database schema.
        modelBuilder.HasDefaultSchema(Schemas.Application);

        // Apply entity configurations.
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseLineConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderLineConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceLineConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Get the current authenticated user's Identity ID.
        string? identityId =
            httpContextAccessor.HttpContext?
                .User
                .GetIdentityId();

        // Track added, modified, and deleted entities.
        var modifiedEntities = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is not AuditLog &&
                (e.State == EntityState.Added ||
                 e.State == EntityState.Modified ||
                 e.State == EntityState.Deleted))
            .ToList();

        foreach (var modifiedEntity in modifiedEntities)
        {
            // Get entity changes.
            string changes = GetChanges(modifiedEntity);

            // Skip modified entities with no actual changes.
            if (modifiedEntity.State == EntityState.Modified &&
                string.IsNullOrWhiteSpace(changes))
            {
                continue;
            }

            // Create the audit log entry.
            AuditLog auditLog = new()
            {
                Id = Guid.CreateVersion7(),
                IdentityId = identityId,
                EntityName = modifiedEntity.Entity.GetType().Name,
                Action = GetAction(modifiedEntity),
                CreatedAtUtc = DateTime.UtcNow,
                Changes = changes
            };

            AuditLogs.Add(auditLog);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    // Builds a summary of entity property changes.
    private static string GetChanges(EntityEntry modifiedEntity)
    {
        StringBuilder changes = new();

        foreach (PropertyEntry property in modifiedEntity.Properties)
        {
            // Ignore unchanged properties.
            if (!property.IsModified ||
                Equals(property.OriginalValue, property.CurrentValue))
            {
                continue;
            }

            // Record the property change.
            changes.AppendLine(
                CultureInfo.InvariantCulture,
                $"{property.Metadata.Name}: From '{property.OriginalValue}' to '{property.CurrentValue}'");
        }

        return changes.ToString();
    }

    // Determines the audit action.
    private static string GetAction(EntityEntry entry)
    {
        // Detect soft delete operations.
        if (entry.State == EntityState.Modified)
        {
            PropertyEntry? isDeletedProperty =
                entry.Properties.FirstOrDefault(
                    p => p.Metadata.Name == nameof(ISoftDeletable.IsDeleted));

            if (isDeletedProperty is not null &&
                Equals(isDeletedProperty.OriginalValue, false) &&
                Equals(isDeletedProperty.CurrentValue, true))
            {
                return "Deleted";
            }
        }

        // Return the EF entity state.
        return entry.State.ToString();
    }
}
