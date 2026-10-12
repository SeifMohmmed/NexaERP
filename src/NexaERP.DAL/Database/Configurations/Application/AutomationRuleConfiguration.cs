using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

internal sealed class AutomationRuleConfiguration
    : IEntityTypeConfiguration<AutomationRule>
{
    public void Configure(EntityTypeBuilder<AutomationRule> builder)
    {
        builder.ToTable("AutomationRules");

        builder.HasKey(rule => rule.Id);

        builder.Property(rule => rule.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(rule => rule.Trigger)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(rule => rule.Action)
            .IsRequired();

        builder.Property(rule => rule.Enabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(rule => rule.CreatedBy)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(rule => rule.CreatedAt)
            .IsRequired();

        builder.Property(rule => rule.UpdatedAt);

        builder.HasIndex(rule => rule.Trigger);

        builder.HasIndex(rule => rule.Enabled);

        builder.HasMany(rule => rule.Runs)
            .WithOne(run => run.Rule)
            .HasForeignKey(run => run.RuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
