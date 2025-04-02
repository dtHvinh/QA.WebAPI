using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.Reports;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetAllReportQuery(PageArgs PageArgs, string? Type = null) :
    IQuery<GenericResult<PagedResponse<GetReportResponse>>>;
