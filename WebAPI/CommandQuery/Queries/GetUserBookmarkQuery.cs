using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.BookmarkResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;
public record GetUserBookmarkQuery(string OrderBy, PageArgs PageArgs) : IQuery<GenericResult<PagedResponse<BookmarkResponse>>>;
