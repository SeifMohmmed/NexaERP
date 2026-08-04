using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class PurchaseOrderConfiguration
    : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        // Configure table name
        builder.ToTable("PurchaseOrders");

        // Configure primary key
        builder.HasKey(po => po.Id);

        // Configure supplier foreign key
        builder.Property(po => po.SupplierId)
            .IsRequired();

        // Configure order date
        builder.Property(po => po.OrderDate)
            .IsRequired();

        // Configure expected delivery date
        builder.Property(po => po.ExpectedDelivery)
            .IsRequired();

        // Configure order status
        builder.Property(po => po.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Configure total amount
        builder.Property(po => po.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(h => h.UserId).HasMaxLength(500);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // Configure Supplier -> PurchaseOrders relationship
        builder.HasOne(po => po.Supplier)
            .WithMany()
            .HasForeignKey(po => po.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
