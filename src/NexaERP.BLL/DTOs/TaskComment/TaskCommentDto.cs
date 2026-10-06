using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.TaskComment;

public sealed class TaskCommentDto
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; } = default!;

    public string Body { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    // Resource links.
    public List<LinkDto> Links { get; set; }
}
