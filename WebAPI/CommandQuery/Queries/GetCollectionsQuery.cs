using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetCollectionsQuery(PageArgs Args, CollectionSortOrder SortOrder) : IQuery<GenericResult<PagedResponse<GetCollectionResponse>>>;