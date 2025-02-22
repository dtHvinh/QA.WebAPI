using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetUserCollectionQuery(PageArgs PageArgs, CollectionSortOrder CollectionSortOrder) : IQuery<GenericResult<PagedResponse<GetCollectionResponse>>>;
