using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class PurchaseLineConfiguration
    : IEntityTypeConfiguration<PurchaseLine>
{
    public void Configure(EntityTypeBuilder<PurchaseLine> builder)
    {
        // Configure table name
        builder.ToTable("PurchaseLines");

        // Configure primary key
        builder.HasKey(pl => pl.Id);

        // Configure purchase order foreign key
        builder.Property(pl => pl.PurchaseOrderId)
            .IsRequired();

        // Configure product foreign key
        builder.Property(pl => pl.ProductId)
            .IsRequired();

        // Configure quantity
        builder.Property(pl => pl.Quantity)
            .IsRequired();

        // Configure unit cost
        builder.Property(pl => pl.UnitCost)
            .IsRequired()
            .HasPrecision(18, 2);

        // Configure PurchaseOrder -> PurchaseLines relationship
        builder.HasOne(pl => pl.PurchaseOrder)
            .WithMany(po => po.Lines)
            .HasForeignKey(pl => pl.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Product -> PurchaseLines relationship
        builder.HasOne(pl => pl.Product)
            .WithMany()
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the row version for optimistic concurrency.
        builder.Property<uint>("Version")
            .IsRowVersion();
    }
}
