using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.VoteResponses;
using WebAPI.Utilities;
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
        var requester = await _userRepository.FindUserByIdAsync(_authContext.UserId, cancellationToken);

        if (request == null)
            return GenericResult<VoteResponse>.Failure("Invalid requester");

        var question = await questionRepository.FindQuestionByIdAsync(request.QuestionId, cancellationToken);

        if (question == null)
        {
            return GenericResult<VoteResponse>.Failure("Question not found");
        }

        if (_authContext.IsResourceOwnedByUser(question))
        {
            return GenericResult<VoteResponse>.Failure("You can't vote your own question");
        }

        var type = request.IsUpvote
            ? await _voteRepository.UpvoteQuestion(request.QuestionId, _authContext.UserId, cancellationToken)
            : await _voteRepository.DownvoteQuestion(request.QuestionId, _authContext.UserId, cancellationToken);

        switch (type)
        {
            case Enums.VoteUpdateTypes.NoChange:
                return GenericResult<VoteResponse>.Failure("You already done this");

            case Enums.VoteUpdateTypes.CreateNew:
                if (request.IsUpvote)
                    question.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.QuestionUpvoted;
                else
                {
                    question.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.QuestionDownvoted;
                    requester!.Reputation += _applicationProperties.ReputationAcquirePerAction.DownvoteQuestion;
                }
                break;

            case Enums.VoteUpdateTypes.ChangeVote:
                if (request.IsUpvote)
                    question.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.QuestionUpvoted;
                else
                {
                    question.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.QuestionDownvoted;
                    requester!.Reputation += _applicationProperties.ReputationAcquirePerAction.DownvoteAnswer;
                }
                break;
        }

        _userRepository.Update(question.Author!);
        questionRepository.VoteChange(question, type, request.IsUpvote ? 1 : -1);

        var result = await questionRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<VoteResponse>.Success(new(question.Upvote, question.Downvote))
            : GenericResult<VoteResponse>.Failure(result.Message);
    }


    public async Task<GenericResult<VoteResponse>> Handle(CreateAnswerVoteCommand request, CancellationToken cancellationToken)
    {
        var answer = await answerRepository.FindAnswerById(request.AnswerId, cancellationToken);

        if (answer == null)
        {
            return GenericResult<VoteResponse>.Failure("Answer not found");
        }

        if (_authContext.IsResourceOwnedByUser(answer))
        {
            return GenericResult<VoteResponse>.Failure("You can't vote your own answer");
        }

        var type = request.IsUpvote
            ? await _voteRepository.UpvoteAnswer(request.AnswerId, _authContext.UserId, cancellationToken)
            : await _voteRepository.DownvoteAnswer(request.AnswerId, _authContext.UserId, cancellationToken);

        switch (type)
        {
            case Enums.VoteUpdateTypes.NoChange:
                return GenericResult<VoteResponse>.Failure("You already done this");

            case Enums.VoteUpdateTypes.CreateNew:
                if (request.IsUpvote)
                    answer.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.AnswerUpvoted;
                else
                    answer.Author!.Reputation -= _applicationProperties.ReputationAcquirePerAction.AnswerDownvoted;
                break;

            case Enums.VoteUpdateTypes.ChangeVote:
                if (request.IsUpvote)
                    answer.Author!.Reputation += _applicationProperties.ReputationAcquirePerAction.AnswerUpvoted;
                else
                    answer.Author!.Reputation -= _applicationProperties.ReputationAcquirePerAction.AnswerDownvoted;
                break;
        }

        answerRepository.VoteChange(answer, type, request.IsUpvote ? 1 : -1);

        var result = await answerRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<VoteResponse>.Success(new(answer.Upvote, answer.Downvote))
            : GenericResult<VoteResponse>.Failure(result.Message);
    }
}

