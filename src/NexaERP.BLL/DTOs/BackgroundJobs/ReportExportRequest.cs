namespace NexaERP.BLL.DTOs.BackgroundJobs;

public sealed record ReportExportRequest(
    string ReportType,
    DateOnly? From,
    DateOnly? To,
    string? GroupBy,
    Guid? CategoryId,
    bool? LowStock,
    Guid? ProjectId,
    Guid? AssigneeId,
    string? Status);
