using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class InvoiceConfiguration
    : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceDate)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(i => i.PaidAt);

        builder.HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(i => i.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(i =>
            !i.IsDeleted &&
            !i.Customer.IsDeleted);

        // Configure the row version for optimistic concurrency.
        builder.Property<uint>("Version")
            .IsRowVersion();
    }
}
