using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class OrderLineConfiguration
    : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        // Configure table name
        builder.ToTable("OrderLines");

        // Configure primary key
        builder.HasKey(ol => ol.Id);

        // Configure order foreign key
        builder.Property(ol => ol.OrderId)
            .IsRequired();

        // Configure product foreign key
        builder.Property(ol => ol.ProductId)
            .IsRequired();

        // Configure quantity
        builder.Property(ol => ol.Quantity)
            .IsRequired();

        // Configure unit price
        builder.Property(ol => ol.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        // Configure discount
        builder.Property(ol => ol.Discount)
            .IsRequired()
            .HasPrecision(18, 2);

        // Configure Order -> OrderLines relationship
        builder.HasOne(ol => ol.Order)
            .WithMany(o => o.Lines)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Product -> OrderLines relationship
        builder.HasOne(ol => ol.Product)
            .WithMany()
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Order -> OrderLines relationship
        builder.HasOne(ol => ol.Order)
            .WithMany(o => o.Lines)
            .HasForeignKey(ol => ol.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Product -> OrderLines relationship
        builder.HasOne(ol => ol.Product)
            .WithMany()
            .HasForeignKey(ol => ol.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure soft delete flag with a default value
        builder.Property(ol => ol.IsDeleted)
            .HasDefaultValue(false);

        // Exclude soft-deleted order lines and lines belonging
        // to filtered orders from queries
        builder.HasQueryFilter(ol =>
            !ol.IsDeleted &&
            !ol.Order.IsDeleted &&
            !ol.Order.Customer.IsDeleted);
    }
}
