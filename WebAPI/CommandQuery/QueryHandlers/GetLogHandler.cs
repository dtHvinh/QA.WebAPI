
using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetLogHandler(ILogRepository logRepository)
    : IQueryHandler<GetLogQuery, GenericResult<PagedResponse<SysLogResponse>>>
{
    private readonly ILogRepository _logRepository = logRepository;

    public async Task<GenericResult<PagedResponse<SysLogResponse>>> Handle(GetLogQuery request, CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetLogs(
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize + 1,
            cancellationToken);

        var hasNext = logs.Count == request.PageArgs.PageSize + 1;

        var totalCount = await _logRepository.CountAsync();

        var response = new PagedResponse<SysLogResponse>(
            logs.Select(e => new SysLogResponse(e.Id, e.Level, e.RenderedMessage, e.UtcTimestamp)).ToList(),
            hasNext,
            request.PageArgs.PageIndex,
            request.PageArgs.PageSize)
        {
            TotalCount = totalCount,
            TotalPage = MathHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
        };

        return GenericResult<PagedResponse<SysLogResponse>>.Success(response);
    }
}
