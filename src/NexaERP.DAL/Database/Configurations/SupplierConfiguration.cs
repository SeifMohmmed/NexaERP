using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations;

internal sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        // Configure table name
        builder.ToTable("Suppliers");

        // Configure primary key
        builder.HasKey(s => s.Id);

        // Configure required properties and their maximum lengths
        builder.Property(s => s.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.ContactName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.PaymentTerms)
            .IsRequired()
            .HasMaxLength(100);

        // Enforce unique email addresses
        builder.HasIndex(s => s.Email)
            .IsUnique();
    }
}
