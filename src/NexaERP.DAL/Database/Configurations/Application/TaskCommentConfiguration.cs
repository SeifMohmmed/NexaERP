using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexaERP.DAL.Entities;

namespace NexaERP.DAL.Database.Configurations.Application;

public sealed class TaskCommentConfiguration
    : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.ToTable("TaskComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Body)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        // Comment -> Task
        builder.HasOne(c => c.Task)
            .WithMany()
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment -> Author
        builder.HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
