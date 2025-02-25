using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record SearchCollectionQuery(string SearchTerm, PageArgs PageArgs) : IQuery<GenericResult<PagedResponse<GetCollectionResponse>>>;
