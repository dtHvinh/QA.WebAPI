using WebAPI.CQRS;
using WebAPI.Response.HistoryResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetQuestionHistoryQuery(int QuestionId) : IQuery<GenericResult<List<QuestionHistoryResponse>>>;
