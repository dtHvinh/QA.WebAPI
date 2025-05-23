namespace WebAPI.Response.Reports;

public record GetReportResponse(
    int Id,
    string Type,
    int TargetId,
    string Description,
    string Status,
    DateTimeOffset CreationDate,
    DateTimeOffset ModificationDate
);