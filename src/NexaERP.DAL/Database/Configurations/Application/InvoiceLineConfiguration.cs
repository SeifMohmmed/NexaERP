using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class InvoiceLineConfiguration
    : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.TaxRate)
            .HasPrecision(5, 2);

        builder.HasOne(i => i.Invoice)
            .WithMany(i => i.Lines)
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(i =>
            !i.IsDeleted &&
            !i.Invoice.IsDeleted &&
            !i.Invoice.Customer.IsDeleted);
    }
}
