using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.ProjectTask;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("tasks")]
[ApiController]
public class TasksController(
    IProjectTaskRepository taskRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    LinkService linkService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    //[HasPermission(Permissions.TasksRead)]
    public async Task<ActionResult<PaginationResult<ProjectTaskDto>>> GetTasks(
        [FromQuery] ProjectTaskQueryParameters query)
    {
        IQueryable<ProjectTaskDto> tasksQuery = taskRepository
            .Search(
                query.ProjectId,
                query.AssigneeId,
                query.Status,
                query.Priority,
                query.DueBefore)
            .Select(ProjectTaskMapping.ProjectToDto());

        var result = await PaginationResult<ProjectTaskDto>.CreateAsync(
            tasksQuery,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
        {
            foreach (var task in result.Items)
            {
                task.Links = CreateLinksForTask(task.Id);
            }

            result.Links = CreateLinksForTasks(
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    //[HasPermission(Permissions.TasksRead)]
    public async Task<ActionResult<ProjectTaskDto>> GetById(
        Guid id,
        [FromQuery] ProjectTaskQueryParameters query)
    {
        ProjectTask? task = await taskRepository
            .GetDetailsByIdAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        ProjectTaskDto dto = task.ToDto();

        if (query.IncludeLinks)
        {
            dto.Links = CreateLinksForTask(dto.Id);
        }

        return Ok(dto);
    }

    [HttpPost]
    //[HasPermission(Permissions.TasksCreate)]
    public async Task<ActionResult<ProjectTaskDto>> Create(
        [FromBody] CreateProjectTaskDto dto,
        [FromServices] IValidator<CreateProjectTaskDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Project? project = await projectRepository
            .GetByIdAsync(dto.ProjectId);

        if (project is null)
        {
            return BadRequest("Project was not found.");
        }

        User? assignee = await userRepository
            .GetByIdAsync(dto.AssigneeId);

        if (assignee is null)
        {
            return BadRequest("Assignee was not found.");
        }

        ProjectTask task = dto.ToEntity();

        await taskRepository.AddAsync(task);

        await unitOfWork.SaveChangesAsync();

        task = await taskRepository
            .GetDetailsByIdAsync(task.Id);

        ProjectTaskDto taskDto = task!.ToDto();

        taskDto.Links = CreateLinksForTask(taskDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = taskDto.Id },
            taskDto);
    }

    [HttpPut("{id:guid}")]
    //[HasPermission(Permissions.TasksUpdate)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectTaskDto dto,
        [FromServices] IValidator<UpdateProjectTaskDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        ProjectTask? task = await taskRepository
            .GetByIdAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        User? assignee = await userRepository
            .GetByIdAsync(dto.AssigneeId);

        if (assignee is null)
        {
            return BadRequest("Assignee was not found.");
        }

        task.UpdateEntity(dto);

        taskRepository.Update(task);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    //[HasPermission(Permissions.TasksUpdateStatus)]
    public async Task<ActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeProjectTaskStatusDto dto,
        [FromServices] IValidator<ChangeProjectTaskStatusDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        ProjectTask? task = await taskRepository
            .GetByIdAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        task.Status = dto.Status;

        taskRepository.Update(task);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    //[HasPermission(Permissions.TasksDelete)]
    public async Task<ActionResult> Delete(Guid id)
    {
        ProjectTask? task = await taskRepository
            .GetByIdAsync(id);

        if (task is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        taskRepository.Delete(task);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForTask(Guid id)
    {
        return
        [
            linkService.Create(
                nameof(GetById),
                "self",
                HttpMethods.Get,
                new { id }),

            linkService.Create(
                nameof(Update),
                "update",
                HttpMethods.Put,
                new { id }),

            linkService.Create(
                nameof(Delete),
                "delete",
                HttpMethods.Delete,
                new { id }),

            linkService.Create(
                nameof(ChangeStatus),
                "change-status",
                HttpMethods.Patch,
                new { id })
        ];
    }

    private List<LinkDto> CreateLinksForTasks(
        ProjectTaskQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(
                nameof(GetTasks),
                "self",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page,
                    pageSize = parameters.PageSize,
                    projectId = parameters.ProjectId,
                    assigneeId = parameters.AssigneeId,
                    status = parameters.Status,
                    priority = parameters.Priority,
                    dueBefore = parameters.DueBefore,
                    includeLinks = parameters.IncludeLinks
                })
        ];

        if (hasNextPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetTasks),
                    "next-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page + 1,
                        pageSize = parameters.PageSize,
                        projectId = parameters.ProjectId,
                        assigneeId = parameters.AssigneeId,
                        status = parameters.Status,
                        priority = parameters.Priority,
                        dueBefore = parameters.DueBefore,
                        includeLinks = parameters.IncludeLinks
                    }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetTasks),
                    "previous-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page - 1,
                        pageSize = parameters.PageSize,
                        projectId = parameters.ProjectId,
                        assigneeId = parameters.AssigneeId,
                        status = parameters.Status,
                        priority = parameters.Priority,
                        dueBefore = parameters.DueBefore,
                        includeLinks = parameters.IncludeLinks
                    }));
        }

        return links;
    }
}
