using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Repositories.Abstraction;
using NexaERP.DAL.Services;

namespace NexaERP.API.Controllers;

[ApiController]
[Authorize]
[Route("jobs")]
public sealed class JobsController(
    IBackgroundJobRepository backgroundJobRepository,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    UserContext userContext) : ControllerBase
{
    [HttpGet("{id:guid}")]
   // [HasPermission(Permissions.ReportsRead)]
    public async Task<IActionResult> GetJob(
        Guid id,
        CancellationToken cancellationToken)
    {
        var job = await backgroundJobRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (job is null)
        {
            return NotFound();
        }

        var currentUserId =
            await userContext.GetUserIdAsync(cancellationToken);

        if (currentUserId is null || currentUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var isOwner = job.RequestedBy == currentUserId.Value.ToString();
        var isAdmin = User.IsInRole("Admin");

        if (!isOwner && !isAdmin)
        {
            return NotFound();
        }

        var downloadUrl = job.Status == "Completed"
            ? Url.Action(
                nameof(Download),
                "Jobs",
                new { id = job.Id },
                Request.Scheme)
            : null;

        return Ok(new
        {
            job.Id,
            job.Type,
            job.Status,
            job.CreatedAt,
            job.CompletedAt,
            job.ExpiresAt,
            downloadUrl
        });
    }

    [HttpGet("{id:guid}/download")]
  //  [HasPermission(Permissions.ReportsRead)]
    public async Task<IActionResult> Download(
        Guid id,
        CancellationToken cancellationToken)
    {
        var job = await backgroundJobRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (job is null)
        {
            return NotFound();
        }

        var currentUserId =
            await userContext.GetUserIdAsync(cancellationToken);

        if (currentUserId is null || currentUserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var isOwner = job.RequestedBy == currentUserId.Value.ToString();
        var isAdmin = User.IsInRole("Admin");

        if (!isOwner && !isAdmin)
        {
            return NotFound();
        }

        if (job.Status != "Completed" ||
            string.IsNullOrWhiteSpace(job.ResultLocation))
        {
            return Conflict(new
            {
                message = "The report is not ready for download."
            });
        }

        if (job.ExpiresAt is null ||
            job.ExpiresAt <= DateTime.UtcNow)
        {
            return NotFound(new
            {
                message = "The report download has expired."
            });
        }

        var directory = configuration["ReportExports:Directory"];

        if (string.IsNullOrWhiteSpace(directory))
        {
            directory = Path.Combine(
                environment.ContentRootPath,
                "App_Data",
                "ReportExports");
        }

        // Resolve the export directory to an absolute path.
        var exportRoot = Path.GetFullPath(directory);

        // Generate the expected filename from the job ID.
        var fileName = $"{job.Id:N}.csv";

        // Resolve the final file path.
        var filePath = Path.GetFullPath(
            Path.Combine(exportRoot, fileName));

        // Ensure the file stays inside the export directory.
        var expectedPrefix = Path.EndsInDirectorySeparator(exportRoot)
            ? exportRoot
            : exportRoot + Path.DirectorySeparatorChar;

        if (!filePath.StartsWith(
                expectedPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return NotFound();
        }

        // Ensure the stored filename matches the expected filename.
        if (!string.Equals(
                job.ResultLocation,
                fileName,
                StringComparison.Ordinal))
        {
            return NotFound();
        }

#pragma warning disable CA3003
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new
            {
                message = "The report file could not be found."
            });
        }

        return PhysicalFile(
            filePath,
            "text/csv; charset=utf-8",
            $"{job.Type}-report-{job.Id:N}.csv");
    }
}
