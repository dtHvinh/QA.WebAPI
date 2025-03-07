using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.VoteResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateVoteHandler(
    IQuestionRepository questionRepository,
    IVoteRepository voteRepository,
    IAnswerRepository answerRepository,
    IUserRepository userRepository,
    AuthenticationContext authContext,
    IOptions<ApplicationProperties> applicationProperties)
    : ICommandHandler<CreateQuestionVoteCommand, GenericResult<VoteResponse>>,
    ICommandHandler<CreateAnswerVoteCommand, GenericResult<VoteResponse>>

{
    private readonly IQuestionRepository questionRepository = questionRepository;
    private readonly IVoteRepository _voteRepository = voteRepository;
    private readonly IAnswerRepository answerRepository = answerRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly ApplicationProperties _applicationProperties = applicationProperties.Value;

    public async Task<GenericResult<VoteResponse>> Handle(CreateQuestionVoteCommand request, CancellationToken cancellationToken)
    {
        var question = await questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (question == null)
        {
            return GenericResult<VoteResponse>.Failure("Question not found");
        }

        if (_authContext.IsResourceOwnedByUser(question))
        {
            return GenericResult<VoteResponse>.Failure("You can't vote for your own question");
        }

        if (request.IsUpvote)
        {
            var res = await _voteRepository.UpvoteQuestion(request.QuestionId, _authContext.UserId, cancellationToken);

            if (res)
            {
                question.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.QuestionUpvoted;

                question.Score++;
            }
            else
                GenericResult<VoteResponse>.Failure("You have already done this");
        }
        else
        {
            var res = await _voteRepository.DownvoteQuestion(request.QuestionId, _authContext.UserId, cancellationToken);

            if (res)
            {
                question.Author!.Reputation -= _applicationProperties.ReputationAcquirePerAction.QuestionDownvoted;

                question.Score--;
            }
            else
                GenericResult<VoteResponse>.Failure("You have already done this");
        }

        _userRepository.Update(question.Author!);

        var result = await questionRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<VoteResponse>.Success(new(question.Score))
            : GenericResult<VoteResponse>.Failure(result.Message);
    }


    public async Task<GenericResult<VoteResponse>> Handle(CreateAnswerVoteCommand request, CancellationToken cancellationToken)
    {
        var answer = await answerRepository.FindAnswerWithAuthorById(request.AnswerId, cancellationToken);

        if (answer == null)
        {
            return GenericResult<VoteResponse>.Failure("Answer not found");
        }

        if (_authContext.IsResourceOwnedByUser(answer))
        {
            return GenericResult<VoteResponse>.Failure("You can't vote for your own answer");
        }

        if (request.IsUpvote)
        {
            var res = await _voteRepository.UpvoteAnswer(request.AnswerId, _authContext.UserId, cancellationToken);

            if (res)
            {
                answer.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.AnswerUpvoted;

                answer.Score++;
            }
            else
                GenericResult<VoteResponse>.Failure("You have already done this");
        }
        else
        {
            var res = await _voteRepository.DownvoteAnswer(request.AnswerId, _authContext.UserId, cancellationToken);

            if (res)
            {
                answer.Author!.Reputation -= _applicationProperties.ReputationAcquirePerAction.AnswerDownvoted;

                answer.Score--;
            }
            else
                GenericResult<VoteResponse>.Failure("You have already done this");
        }

        _userRepository.Update(answer.Author!);

        var result = await answerRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<VoteResponse>.Success(new(answer.Score))
            : GenericResult<VoteResponse>.Failure(result.Message);
    }
}

