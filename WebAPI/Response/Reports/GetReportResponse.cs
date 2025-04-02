namespace WebAPI.Response.Reports;

public record GetReportResponse(
    int Id,
    string Type,
    int TargetId,
    string Description,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);