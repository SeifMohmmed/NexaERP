using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NexaERP.API.Settings;
using NexaERP.BLL.DTOs.BackgroundJobs;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.API.BackgroundJobs;

public sealed class ReportExportJob(
    IBackgroundJobRepository backgroundJobRepository,
    IUnitOfWork unitOfWork,
    ISalesReportRepository salesReportRepository,
    IInventoryReportRepository inventoryReportRepository,
    IInvoiceReportRepository invoiceReportRepository,
    ITaskReportRepository taskReportRepository,
    IOptions<CurrencyOptions> currencyOptions,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    ILogger<ReportExportJob> logger)
{
    public async Task GenerateCsvAsync(Guid backgroundJobId)
    {
        var job = await backgroundJobRepository.GetByIdAsync(backgroundJobId)
            ?? throw new InvalidOperationException(
                $"Background job '{backgroundJobId}' was not found.");

        try
        {
            job.Status = "Processing";
            backgroundJobRepository.Update(job);
            await unitOfWork.SaveChangesAsync();

            var request = JsonSerializer.Deserialize<ReportExportRequest>(
                job.Parameters
                ?? throw new InvalidOperationException(
                    "Export parameters are missing."));

            if (request is null)
            {
                throw new InvalidOperationException(
                    "Invalid export parameters.");
            }

            var csv = await GenerateCsvContentAsync(request);

            var directory = configuration["ReportExports:Directory"];

            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Path.Combine(
                    environment.ContentRootPath,
                    "App_Data",
                    "ReportExports");
            }

            Directory.CreateDirectory(directory);

            var fileName = $"{job.Id:N}.csv";
            var filePath = Path.Combine(directory, fileName);

            await File.WriteAllTextAsync(
                filePath,
                csv,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

            job.ResultLocation = fileName;
            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.ExpiresAt = DateTime.UtcNow.AddHours(24);

            backgroundJobRepository.Update(job);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to generate CSV for background job {BackgroundJobId}.",
                backgroundJobId);

            job.Status = "Failed";
            job.CompletedAt = DateTime.UtcNow;

            backgroundJobRepository.Update(job);
            await unitOfWork.SaveChangesAsync();

            throw;
        }
    }

    private async Task<string> GenerateCsvContentAsync(
        ReportExportRequest request)
    {
        var builder = new StringBuilder();

        switch (request.ReportType.ToLowerInvariant())
        {
            case "sales":
            {
                var report = await salesReportRepository.GetSalesReportAsync(
                    request.From,
                    request.To,
                    request.GroupBy!,
                    CancellationToken.None);

                AppendRow(builder,
                    "Period", "TotalSales", "OrderCount", "Currency");

                foreach (var item in report)
                {
                    AppendRow(builder,
                        item.Period,
                        item.TotalSales,
                        item.OrderCount,
                        currencyOptions.Value.BaseCurrency);
                }

                break;
            }

            case "inventory":
            {
                var report =
                    await inventoryReportRepository.GetInventoryReportAsync(
                        request.CategoryId,
                        request.LowStock,
                        CancellationToken.None);

                AppendRow(builder,
                    "ProductId", "ProductName", "SKU", "CategoryId",
                    "StockQuantity", "ReorderLevel", "IsLowStock");

                foreach (var item in report)
                {
                    AppendRow(builder,
                        item.ProductId,
                        item.ProductName,
                        item.SKU,
                        item.CategoryId,
                        item.StockQuantity,
                        item.ReorderLevel,
                        item.IsLowStock);
                }

                break;
            }

            case "invoices":
            {
                var report =
                    await invoiceReportRepository.GetInvoiceReportAsync(
                        request.From,
                        request.To,
                        request.Status,
                        CancellationToken.None);

                AppendRow(builder,
                    "InvoiceId", "InvoiceDate", "DueDate", "Status",
                    "TotalAmount", "IsOverdue", "OverdueDays");

                foreach (var item in report)
                {
                    AppendRow(builder,
                        item.InvoiceId,
                        item.InvoiceDate,
                        item.DueDate,
                        item.Status,
                        item.TotalAmount,
                        item.IsOverdue,
                        item.OverdueDays);
                }

                break;
            }

            case "tasks":
            {
                var report = await taskReportRepository.GetTaskReportAsync(
                    request.ProjectId,
                    request.AssigneeId,
                    request.From,
                    request.To,
                    request.Status,
                    CancellationToken.None);

                AppendRow(builder,
                    "ProjectId", "AssigneeId", "Status", "TaskCount");

                foreach (var item in report)
                {
                    AppendRow(builder,
                        item.ProjectId,
                        item.AssigneeId,
                        item.Status,
                        item.TaskCount);
                }

                break;
            }

            default:
                throw new InvalidOperationException(
                    $"Unsupported report type '{request.ReportType}'.");
        }

        return builder.ToString();
    }

    private static void AppendRow(
        StringBuilder builder,
        params object?[] values)
    {
        builder.AppendLine(string.Join(",",
            values.Select(value => EscapeCsv(FormatValue(value)))));
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => string.Empty,
            DateTime dateTime => dateTime.ToString(
                "O", CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString(
                "O", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(
                null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    private static string EscapeCsv(string value)
    {
        if (!value.Contains(',') &&
            !value.Contains('"') &&
            !value.Contains('\r') &&
            !value.Contains('\n'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
