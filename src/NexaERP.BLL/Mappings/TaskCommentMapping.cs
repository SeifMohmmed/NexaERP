using System.Linq.Expressions;
using NexaERP.BLL.DTOs.TaskComment;
using NexaERP.DAL.Entities;

namespace NexaERP.BLL.Mappings;

public static class TaskCommentMapping
{
    public static TaskCommentDto ToDto(this TaskComment comment)
    {
        return new TaskCommentDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            AuthorName = $"{comment.Author.FirstName} {comment.Author.LastName}",
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static Expression<Func<TaskComment, TaskCommentDto>> ProjectToDto()
    {
        return comment => new TaskCommentDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            AuthorName =
                comment.Author.FirstName + " " + comment.Author.LastName,
            Body = comment.Body,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static TaskComment ToEntity(
        this CreateTaskCommentDto dto,
        Guid taskId,
        Guid authorId)
    {
        return new TaskComment
        {
            TaskId = taskId,
            AuthorId = authorId,
            Body = dto.Body,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(
        this TaskComment comment,
        UpdateTaskCommentDto dto)
    {
        comment.Body = dto.Body;
        comment.UpdatedAt = DateTime.UtcNow;
    }
}
