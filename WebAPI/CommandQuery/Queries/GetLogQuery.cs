using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetLogQuery(PageArgs PageArgs) : IQuery<GenericResult<PagedResponse<SysLogResponse>>>;
