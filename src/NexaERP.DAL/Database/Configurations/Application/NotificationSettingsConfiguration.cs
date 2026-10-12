using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

public sealed class NotificationSettingsConfiguration
    : IEntityTypeConfiguration<NotificationSettings>
{
    public void Configure(
        EntityTypeBuilder<NotificationSettings> builder)
    {
        builder.ToTable("NotificationSettings");

        builder.HasKey(s => new
        {
            s.UserId,
            s.Channel,
            s.EventType
        });

        builder.Property(s => s.Channel)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Enabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new
        {
            s.UserId,
            s.Channel,
            s.Enabled
        });
    }
}
