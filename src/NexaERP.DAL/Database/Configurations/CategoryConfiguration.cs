using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Maps the entity to the Categories table.
        builder.ToTable("Categories");

        // Configures the primary key.
        builder.HasKey(c => c.Id);

        // Category name is required and must be unique.
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Prevents duplicate category names.
        builder.HasIndex(c => c.Name)
            .IsUnique();

        // Optional description with a maximum length.
        builder.Property(c => c.Description)
            .HasMaxLength(500);
    }
}
