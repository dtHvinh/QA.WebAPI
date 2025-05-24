using Riok.Mapperly.Abstractions;
using WebAPI.Model;
using WebAPI.Response.Reports;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class ReportExtensions
{
    public static partial GetReportResponse ToGetReportResponse(this Report report);
}
