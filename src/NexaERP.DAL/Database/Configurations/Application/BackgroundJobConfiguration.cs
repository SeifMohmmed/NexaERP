using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

public class BackgroundJobConfiguration
    : IEntityTypeConfiguration<BackgroundJob>
{
    public void Configure(EntityTypeBuilder<BackgroundJob> builder)
    {
        builder.ToTable("BackgroundJobs");

        builder.HasKey(job => job.Id);

        builder.Property(job => job.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(job => job.RequestedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(job => job.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(job => job.Parameters)
            .HasColumnType("jsonb");

        builder.Property(job => job.ResultLocation)
            .HasMaxLength(1000);

        builder.Property(job => job.HangfireJobId)
            .HasMaxLength(100);

        builder.HasIndex(job => new
        {
            job.RequestedBy,
            job.CreatedAt
        });

        builder.HasIndex(job => job.Status);
    }
}
