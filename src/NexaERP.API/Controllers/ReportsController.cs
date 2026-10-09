
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using NexaERP.API.BackgroundJobs;
using NexaERP.API.Settings;
using NexaERP.BLL.DTOs.BackgroundJobs;
using NexaERP.BLL.DTOs.InventoryReport;
using NexaERP.BLL.DTOs.InvoiceReport;
using NexaERP.BLL.DTOs.ProjectTaskReport;
using NexaERP.BLL.DTOs.SalesReport;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Extensions;
using NexaERP.DAL.Repositories.Abstraction;
using NexaERP.DAL.Services;
using BackgroundJob = NexaERP.DAL.Entities.BackgroundJob;

namespace NexaERP.API.Controllers;

[EnableRateLimiting(RateLimitingPolicies.Default)]
[Authorize]
[Route("reports")]
[ApiController]
#pragma warning disable S6960
public sealed class ReportsController(
    ISalesReportRepository salesReportRepository,
    IInventoryReportRepository inventoryReportRepository,
    IInvoiceReportRepository invoiceReportRepository,
    ITaskReportRepository taskReportRepository,
    IBackgroundJobRepository backgroundJobRepository,
    IUnitOfWork unitOfWork,
    IBackgroundJobClient backgroundJobClient,
    UserContext userContext,
    IOptions<CurrencyOptions> currencyOptions) : ControllerBase
{
    [HttpGet("sales")]
    //[HasPermission(Permissions.ReportsRead)]
    public async Task<ActionResult<List<SalesReportItemDto>>> GetSalesReport(
        [FromQuery] SalesReportQueryParameters query,
        [FromServices] IValidator<SalesReportQueryParameters> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);

        List<SalesReportItem> report =
            await salesReportRepository.GetSalesReportAsync(
                query.From,
                query.To,
                query.GroupBy,
                cancellationToken);

        string currency = currencyOptions.Value.BaseCurrency;

        var result = report
            .Select(item => new SalesReportItemDto
            {
                Period = item.Period,
                TotalSales = item.TotalSales,
                OrderCount = item.OrderCount,
                Currency = currency
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("inventory")]
    //[HasPermission(Permissions.ReportsRead)]
    public async Task<ActionResult<List<InventoryReportItemDto>>> GetInventoryReport(
        [FromQuery] InventoryReportQueryParameters query,
        CancellationToken cancellationToken)
    {
        List<InventoryReportItem> report =
            await inventoryReportRepository.GetInventoryReportAsync(
                query.CategoryId,
                query.LowStock,
                cancellationToken);

        var result = report
            .Select(item => new InventoryReportItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                SKU = item.SKU,
                CategoryId = item.CategoryId,
                StockQuantity = item.StockQuantity,
                ReorderLevel = item.ReorderLevel,
                IsLowStock = item.IsLowStock
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("invoices")]
    //[HasPermission(Permissions.ReportsRead)]
    public async Task<ActionResult<List<InvoiceReportItemDto>>> GetInvoiceReport(
        [FromQuery] InvoiceReportQueryParameters query,
        [FromServices] IValidator<InvoiceReportQueryParameters> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);

        List<InvoiceReportItem> report =
            await invoiceReportRepository.GetInvoiceReportAsync(
                query.From,
                query.To,
                query.Status,
                cancellationToken);

        var result = report
            .Select(item => new InvoiceReportItemDto
            {
                InvoiceId = item.InvoiceId,
                InvoiceDate = item.InvoiceDate,
                DueDate = item.DueDate,
                Status = item.Status,
                TotalAmount = item.TotalAmount,
                IsOverdue = item.IsOverdue,
                OverdueDays = item.OverdueDays
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("tasks")]
    //[HasPermission(Permissions.ReportsRead)]
    public async Task<ActionResult<List<TaskReportItemDto>>> GetTaskReport(
        [FromQuery] TaskReportQueryParameters query,
        CancellationToken cancellationToken)
    {
        var report = await taskReportRepository.GetTaskReportAsync(
            query.ProjectId,
            query.AssigneeId,
            query.From,
            query.To,
            query.Status,
            cancellationToken);

        var result = report
            .Select(item => new TaskReportItemDto
            {
                ProjectId = item.ProjectId,
                AssigneeId = item.AssigneeId,
                Status = item.Status,
                TaskCount = item.TaskCount
            })
            .ToList();

        return Ok(result);
    }

    [HttpPost("exports")]
   // [HasPermission(Permissions.ReportsRead)]
    public async Task<IActionResult> CreateExport(
        [FromBody] ReportExportRequest request,
        CancellationToken cancellationToken)
    {
        var requestedBy = await userContext.GetUserIdAsync(
            cancellationToken);

        if (requestedBy is null || requestedBy == Guid.Empty)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.ReportType))
        {
            return BadRequest(new
            {
                message = "Report type is required."
            });
        }

        var reportType = request.ReportType
            .Trim()
            .ToLowerInvariant();

        var allowedReportTypes = new[]
        {
            "sales",
            "inventory",
            "invoices",
            "tasks"
        };

        if (!allowedReportTypes.Contains(reportType))
        {
            return BadRequest(new
            {
                message = "Unsupported report type."
            });
        }

        if (request.From.HasValue &&
            request.To.HasValue &&
            request.From.Value > request.To.Value)
        {
            return BadRequest(new
            {
                message = "'From' must be earlier than or equal to 'To'."
            });
        }

        var normalizedRequest = request with
        {
            ReportType = reportType
        };

        var backgroundJob = new BackgroundJob
        {
            Type = reportType,
            RequestedBy = requestedBy.Value.ToString(),
            Status = "Queued",
            Parameters = JsonSerializer.Serialize(normalizedRequest),
            CreatedAt = DateTime.UtcNow
        };

        // Persist the job record before enqueueing it.
        await backgroundJobRepository.AddAsync(backgroundJob);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var hangfireJobId =
                backgroundJobClient.Enqueue<ReportExportJob>(
                    job => job.GenerateCsvAsync(backgroundJob.Id));

            backgroundJob.HangfireJobId = hangfireJobId;

            backgroundJobRepository.Update(backgroundJob);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            backgroundJob.Status = "Failed";
            backgroundJob.CompletedAt = DateTime.UtcNow;

            backgroundJobRepository.Update(backgroundJob);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unable to queue the report export.");
        }

        var statusUrl = Url.Action(
            action: "GetJob",
            controller: "Jobs",
            values: new { id = backgroundJob.Id },
            protocol: Request.Scheme);

        return Accepted(new
        {
            id = backgroundJob.Id,
            status = backgroundJob.Status,
            statusUrl
        });
    }
}
