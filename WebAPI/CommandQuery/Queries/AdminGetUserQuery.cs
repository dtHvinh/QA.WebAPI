using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record AdminGetUserQuery(PageArgs PageArgs) : IQuery<GenericResult<PagedResponse<GetUserResponse>>>;
