using WebAPI.CQRS;
using WebAPI.Response.VoteResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateAnswerVoteCommand(Guid AnswerId, bool IsUpvote) : ICommand<GenericResult<VoteResponse>>;
