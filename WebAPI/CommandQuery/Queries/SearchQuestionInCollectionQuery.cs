using WebAPI.CQRS;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record SearchQuestionInCollectionQuery(int CollectionId, string SearchTerm) : IQuery<GenericResult<List<GetQuestionResponse>>>;
