using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations;

internal sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Configure table name
        builder.ToTable("Orders");

        // Configure primary key
        builder.HasKey(o => o.Id);

        // Configure customer foreign key
        builder.Property(o => o.CustomerId)
            .IsRequired();

        // Configure order date
        builder.Property(o => o.OrderDate)
            .IsRequired();

        // Configure order status
        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Configure payment method
        builder.Property(o => o.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);

        // Configure shipping address
        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(250);

        // Configure total amount
        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        // Configure Customer -> Orders relationship
        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure soft delete flag with a default value
        builder.Property(o => o.IsDeleted)
            .HasDefaultValue(false);

        // Exclude soft-deleted orders and orders belonging to
        // soft-deleted customers from queries
        builder.HasQueryFilter(o =>
            !o.IsDeleted &&
            !o.Customer.IsDeleted);
    }
}
