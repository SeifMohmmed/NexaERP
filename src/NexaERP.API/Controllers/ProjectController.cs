using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Common;
using NexaERP.BLL.DTOs.Project;
using NexaERP.BLL.Mappings;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("projects")]
[ApiController]
public class ProjectsController(
    IProjectRepository projectRepository,
    IProjectTaskRepository taskRepository,
    IUserRepository userRepository,
    LinkService linkService,
    IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    //[HasPermission(Permissions.ProjectsRead)]
    public async Task<ActionResult<PaginationResult<ProjectDto>>> GetProjects(
        [FromQuery] ProjectQueryParameters query)
    {
        IQueryable<ProjectDto> projectsQuery = projectRepository
            .Search(
                query.Search,
                query.Status,
                query.OwnerId)
            .Select(ProjectMapping.ProjectToDto());

        var result = await PaginationResult<ProjectDto>.CreateAsync(
            projectsQuery,
            query.Page,
            query.PageSize);

        if (query.IncludeLinks)
        {
            foreach (var project in result.Items)
            {
                project.Links = CreateLinksForProject(project.Id);
            }

            result.Links = CreateLinksForProjects(
                query,
                result.HasNextPage,
                result.HasPreviousPage);
        }

        return Ok(result);
    }
    [HttpGet("{id:guid}")]
   // [HasPermission(Permissions.ProjectsRead)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id,
        [FromQuery] ProjectQueryParameters query)
    {
        Project? project = await projectRepository
            .GetDetailsByIdAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        ProjectDto dto = project.ToDto();

        if (query.IncludeLinks)
        {
            dto.Links = CreateLinksForProject(dto.Id);
        }        
        return Ok(dto);
    }

    [HttpPost]
   // [HasPermission(Permissions.ProjectsCreate)]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromBody] CreateProjectDto dto,
        [FromServices] IValidator<CreateProjectDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        User? owner = await userRepository.GetByIdAsync(dto.OwnerId);

        if (owner is null)
        {
            return BadRequest("Project owner was not found.");
        }

        Project project = dto.ToEntity();

        await projectRepository.AddAsync(project);

        await unitOfWork.SaveChangesAsync();

        ProjectDto projectDto = project.ToDto();

        projectDto.Links = CreateLinksForProject(projectDto.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = projectDto.Id },
            projectDto);
    }

    [HttpPut("{id:guid}")]
   // [HasPermission(Permissions.ProjectsUpdate)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectDto dto,
        [FromServices] IValidator<UpdateProjectDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Project? project = await projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        User? owner = await userRepository.GetByIdAsync(dto.OwnerId);

        if (owner is null)
        {
            return BadRequest("Project owner was not found.");
        }

        project.UpdateEntity(dto);

        projectRepository.Update(project);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    //[HasPermission(Permissions.ProjectsUpdate)]
    public async Task<ActionResult> ChangeStatus(
        Guid id,
        [FromBody] ChangeProjectStatusDto dto,
        [FromServices] IValidator<ChangeProjectStatusDto> validator)
    {
        await validator.ValidateAndThrowAsync(dto);

        Project? project = await projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        project.Status = dto.Status;

        projectRepository.Update(project);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    //[HasPermission(Permissions.ProjectsDelete)]
    public async Task<ActionResult> Delete(Guid id)
    {
        Project? project = await projectRepository.GetByIdAsync(id);

        if (project is null)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }

        bool hasActiveTasks = await taskRepository.ExistsAsync(
            task =>
                task.ProjectId == id &&
                task.Status != NexaERP.DAL.Enums.ProjectTaskStatus.Done);

        if (hasActiveTasks)
        {
            return Conflict(
                "Project cannot be deleted while it has active tasks.");
        }

        projectRepository.Delete(project);

        await unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    private List<LinkDto> CreateLinksForProject(Guid id)
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
    
    private List<LinkDto> CreateLinksForProjects(
        ProjectQueryParameters parameters,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            linkService.Create(
                nameof(GetProjects),
                "self",
                HttpMethods.Get,
                new
                {
                    page = parameters.Page,
                    pageSize = parameters.PageSize,
                    search = parameters.Search,
                    status = parameters.Status,
                    ownerId = parameters.OwnerId,
                    includeLinks = parameters.IncludeLinks
                })
        ];

        if (hasNextPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetProjects),
                    "next-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page + 1,
                        pageSize = parameters.PageSize,
                        search = parameters.Search,
                        status = parameters.Status,
                        ownerId = parameters.OwnerId,
                        includeLinks = parameters.IncludeLinks
                    }));
        }

        if (hasPreviousPage)
        {
            links.Add(
                linkService.Create(
                    nameof(GetProjects),
                    "previous-page",
                    HttpMethods.Get,
                    new
                    {
                        page = parameters.Page - 1,
                        pageSize = parameters.PageSize,
                        search = parameters.Search,
                        status = parameters.Status,
                        ownerId = parameters.OwnerId,
                        includeLinks = parameters.IncludeLinks
                    }));
        }

        return links;
    }
}
