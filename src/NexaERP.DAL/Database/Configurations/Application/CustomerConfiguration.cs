using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Configure table name
        builder.ToTable("Customers");

        // Configure primary key
        builder.HasKey(c => c.Id);

        // Configure required properties and their maximum lengths
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Country)
            .IsRequired()
            .HasMaxLength(100);

        // Tax ID is optional
        builder.Property(c => c.TaxId)
            .HasMaxLength(50);

        // Enforce unique email addresses
        builder.HasIndex(c => c.Email)
            .IsUnique();

        // Enforce unique tax IDs
        builder.HasIndex(c => c.TaxId)
            .IsUnique();

        // Configure soft delete flag with a default value
        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        // Exclude soft-deleted customers from queries
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
