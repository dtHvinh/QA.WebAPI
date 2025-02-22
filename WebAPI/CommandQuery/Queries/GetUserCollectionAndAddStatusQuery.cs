using WebAPI.CQRS;
using WebAPI.Response.CollectionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetUserCollectionAndAddStatusQuery(int QuestionId) : IQuery<GenericResult<List<GetCollectionWithAddStatusResponse>>>;
