using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetUserCollectionAndAddStatusQuery(int QuestionId, PageArgs PageArgs) : IQuery<GenericResult<PagedResponse<GetCollectionWithAddStatusResponse>>>;
