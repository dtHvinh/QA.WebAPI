using WebAPI.Model;
using WebAPI.Response.Reports;

namespace WebAPI.Utilities.Extensions;

public static class ReportExtensions
{
    public static GetReportResponse ToGetReportResponse(this Report report)
    {
        return new GetReportResponse(report.Id, report.Type, report.TargetId, report.Description, report.Status, report.CreatedAt, report.UpdatedAt);
    }
}
