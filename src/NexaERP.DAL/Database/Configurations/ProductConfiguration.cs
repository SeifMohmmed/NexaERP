using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Maps the entity to the Products table.
        builder.ToTable("Products");

        // Configures the primary key.
        builder.HasKey(p => p.Id);

        // Product name is required and limited to 200 characters.
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // SKU is required, has a maximum length, and must be unique.
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        // Prevents duplicate SKUs.
        builder.HasIndex(p => p.SKU)
            .IsUnique();

        // Stores selling price with two decimal places.
        builder.Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

        // Stores cost price with two decimal places.
        builder.Property(p => p.CostPrice)
            .HasPrecision(18, 2);

        // Initializes stock quantity to zero when not provided.
        builder.Property(p => p.StockQuantity)
            .HasDefaultValue(0);

        // Initializes the reorder level to zero when not provided.
        builder.Property(p => p.ReorderLevel)
            .HasDefaultValue(0);

        // Products are active by default (not soft-deleted).
        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);

        // Configures the relationship between Product and Category.
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a category that is referenced by products.
    }
}
