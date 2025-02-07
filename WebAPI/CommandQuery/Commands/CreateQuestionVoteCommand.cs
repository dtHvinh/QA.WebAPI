using WebAPI.CQRS;
using WebAPI.Response.VoteResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateQuestionVoteCommand(int QuestionId, bool IsUpvote)
    : ICommand<GenericResult<VoteResponse>>;
