using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateVoteHandler(
    IQuestionRepository questionRepository,
    IVoteRepository voteRepository,
    AuthenticationContext authContext)
    : ICommandHandler<CreateQuestionVoteCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository questionRepository = questionRepository;
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<GenericResponse>> Handle(CreateQuestionVoteCommand request, CancellationToken cancellationToken)
    {
        var question = await questionRepository.FindQuestionByIdAsync(request.QuestionId, cancellationToken);

        if (question == null)
        {
            return GenericResult<GenericResponse>.Failure("Question not found");
        }

        var type = request.IsUpvote
            ? await _voteRepository.UpvoteQuestion(request.QuestionId, _authContext.UserId, cancellationToken)
            : await _voteRepository.DownvoteQuestion(request.QuestionId, _authContext.UserId, cancellationToken);

        switch (type)
        {
            case Enums.VoteUpdateTypes.NoChange:
                return GenericResult<GenericResponse>.Failure("You already done this");
        }

        questionRepository.VoteChange(question, type, request.IsUpvote ? 1 : -1);

        var result = await questionRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new())
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
