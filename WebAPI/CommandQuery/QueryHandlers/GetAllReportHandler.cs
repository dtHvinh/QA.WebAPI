using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.Reports;
using WebAPI.Utilities;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetAllReportHandler(
    IReportRepository reportRepository)
    : IQueryHandler<GetAllReportQuery, GenericResult<PagedResponse<GetReportResponse>>>
{
    private readonly IReportRepository _reportRepository = reportRepository;

    public async Task<GenericResult<PagedResponse<GetReportResponse>>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.FindAllReport(request.Type, request.PageArgs.CalculateSkip(), request.PageArgs.PageSize + 1, cancellationToken);

        var hasNext = reports.Count > request.PageArgs.PageSize;

        var total = await _reportRepository.CountAsync();

        return GenericResult<PagedResponse<GetReportResponse>>.Success(
            new PagedResponse<GetReportResponse>(
                reports.Take(request.PageArgs.PageSize).Select(e => e.ToGetReportResponse()),
                hasNext,
                request.PageArgs.PageIndex,
                request.PageArgs.PageSize)
            {
                TotalCount = total,
                TotalPage = MathHelper.GetTotalPage(total, request.PageArgs.PageSize)
            });
    }
}
