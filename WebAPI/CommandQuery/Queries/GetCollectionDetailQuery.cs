using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetCollectionDetailQuery(int Id, PageArgs PageArgs) : IQuery<GenericResult<GetCollectionDetailResponse>>;
