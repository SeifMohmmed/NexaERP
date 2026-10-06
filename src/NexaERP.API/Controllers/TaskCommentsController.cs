using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.TaskComment;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Identity;
using NexaERP.DAL.Repositories.Abstraction;
using NexaERP.DAL.Services;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("tasks/{taskId:guid}/comments")]
[ApiController]
public class TaskCommentsController(
    ITaskCommentRepository commentRepository,
    IProjectTaskRepository taskRepository,
    UserContext userContext,
    LinkService linkService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
   // [HasPermission(Permissions.TasksRead)]
    public async Task<ActionResult<PaginationResult<TaskCommentDto>>> GetComments(
        Guid taskId,
        [FromQuery] TaskCommentQueryParameters query)
    {
        ProjectTask? task =
            await taskRepository.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound();
        }

        IQueryable<TaskCommentDto> commentsQuery =
            commentRepository
                .GetByTaskId(taskId)
                .Select(TaskCommentMapping.ProjectToDto());

        var result =
            await PaginationResult<TaskCommentDto>.CreateAsync(
                commentsQuery,
                query.Page,
                query.PageSize);

        if (query.IncludeLinks)
        {
            foreach (TaskCommentDto comment in result.Items)
            {
                comment.Links = CreateLinksForComment(
                    taskId,
                    comment.Id);
            }

            result.Links = CreateLinksForComments(
                taskId,
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TaskCommentDto>> Create(
        Guid taskId,
        [FromBody] CreateTaskCommentDto dto,
        [FromServices] IValidator<CreateTaskCommentDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        ProjectTask? task =
            await taskRepository.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound("Task was not found.");
        }

        Guid? currentUserId =
            await userContext.GetUserIdAsync();

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        TaskComment comment = dto.ToEntity(
            taskId,
            currentUserId.Value);

        await commentRepository.AddAsync(comment);

        await unitOfWork.SaveChangesAsync();

        comment =
            await commentRepository.GetDetailsByIdAsync(comment.Id);

        TaskCommentDto result = comment!.ToDto();

        result.Links = CreateLinksForComment(
            taskId,
            result.Id);

        return CreatedAtAction(
            nameof(GetComments),
            new { taskId },
            result);
    }

    [HttpPut("{commentId:guid}")]
    public async Task<ActionResult> Update(
        Guid taskId,
        Guid commentId,
        [FromBody] UpdateTaskCommentDto dto,
        [FromServices] IValidator<UpdateTaskCommentDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        TaskComment? comment =
            await commentRepository.GetDetailsByIdAsync(commentId);

        if (comment is null || comment.TaskId != taskId)
        {
            return NotFound();
        }

        Guid? currentUserId =
            await userContext.GetUserIdAsync();

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        bool isAuthor =
            comment.AuthorId == currentUserId.Value;

        bool isProjectOwner =
            comment.Task.Project.OwnerId == currentUserId.Value;

        bool isAdmin =
            User.IsInRole(Roles.Admin);

        if (!isAuthor && !isProjectOwner && !isAdmin)
        {
            return Forbid();
        }

        comment.UpdateEntity(dto);

        commentRepository.Update(comment);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<ActionResult> Delete(
        Guid taskId,
        Guid commentId)
    {
        TaskComment? comment =
            await commentRepository.GetDetailsByIdAsync(commentId);

        if (comment is null || comment.TaskId != taskId)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        Guid? currentUserId =
            await userContext.GetUserIdAsync();

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        bool isAuthor =
            comment.AuthorId == currentUserId.Value;

        bool isProjectOwner =
            comment.Task.Project.OwnerId == currentUserId.Value;

        bool isAdmin =
            User.IsInRole(Roles.Admin);

        if (!isAuthor && !isProjectOwner && !isAdmin)
        {
            return Forbid();
        }

        commentRepository.Delete(comment);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForComments(
        Guid taskId,
        TaskCommentQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(
                nameof(GetComments),
                "self",
                HttpMethods.Get,
                new
                {
                    taskId,
                    page = parameters.Page,
                    pageSize = parameters.PageSize,
                    includeLinks = parameters.IncludeLinks
                }),

            linkService.Create(
                nameof(Create),
                "create-comment",
                HttpMethods.Post,
                new { taskId })
        ];

        if (hasNextPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetComments),
                    "next-page",
                    HttpMethods.Get,
                    new
                    {
                        taskId,
                        page = parameters.Page + 1,
                        pageSize = parameters.PageSize,
                        includeLinks = parameters.IncludeLinks
                    }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetComments),
                    "previous-page",
                    HttpMethods.Get,
                    new
                    {
                        taskId,
                        page = parameters.Page - 1,
                        pageSize = parameters.PageSize,
                        includeLinks = parameters.IncludeLinks
                    }));
        }

        return links;
    }

    private List<LinkDto> CreateLinksForComment(
        Guid taskId,
        Guid commentId)
    {
        return
        [
            linkService.Create(
                nameof(GetComments),
                "comments",
                HttpMethods.Get,
                new { taskId }),

            linkService.Create(
                nameof(Update),
                "update",
                HttpMethods.Put,
                new { taskId, commentId }),

            linkService.Create(
                nameof(Delete),
                "delete",
                HttpMethods.Delete,
                new { taskId, commentId })
        ];
    }
}
