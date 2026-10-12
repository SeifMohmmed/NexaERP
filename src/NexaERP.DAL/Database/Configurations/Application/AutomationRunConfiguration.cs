using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class AutomationRunConfiguration
    : IEntityTypeConfiguration<AutomationRun>
{
    public void Configure(EntityTypeBuilder<AutomationRun> builder)
    {
        builder.ToTable("AutomationRuns");

        builder.HasKey(run => run.Id);

        builder.Property(run => run.EventId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(run => run.Trigger)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(run => run.Status)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Queued");

        builder.Property(run => run.StartedAt)
            .IsRequired();

        builder.Property(run => run.CompletedAt);

        builder.Property(run => run.ErrorSummary)
            .HasMaxLength(2000);
        
        // Supports filtering execution history by rule and status.
        builder.HasIndex(run => new { run.RuleId, run.Status });

        // Supports filtering executions by event.
        builder.HasIndex(run => run.EventId);

        // Prevents duplicate runs for the same rule and business event.
        builder.HasIndex(run => new { run.RuleId, run.EventId })
            .IsUnique();

    }
}
