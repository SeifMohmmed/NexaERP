using NexaERP.BLL.DTOs.Common;

namespace NexaERP.BLL.DTOs.TaskComment;

public sealed class TaskCommentQueryParameters  : AcceptHeaderDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

}
