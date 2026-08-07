using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Configure table name.
        builder.ToTable("AuditLogs");

        // Configure primary key.
        builder.HasKey(x => x.Id);

        // Configure entity name.
        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        // Configure action name.
        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(50);

        // Configure change details.
        builder.Property(x => x.Changes)
            .IsRequired();

        // Configure audit timestamp.
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Configure Identity user ID.
        builder.Property(x => x.IdentityId)
            .HasMaxLength(450);

        // Configure indexes.
        builder.HasIndex(x => x.IdentityId);
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasIndex(x => x.EntityName);
    }
}
