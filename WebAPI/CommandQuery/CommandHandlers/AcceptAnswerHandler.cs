using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class AcceptAnswerHandler(IQuestionRepository questionRepository,
                                 IAnswerRepository answerRepository,
                                 IUserRepository userRepository,
                                 AuthenticationContext authenticationContext,
                                 IOptions<ApplicationProperties> options)
    : ICommandHandler<AcceptAnswerCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly ApplicationProperties options = options.Value;

    public async Task<GenericResult<GenericResponse>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        if (question.IsSolved)
            return GenericResult<GenericResponse>.Failure("Question is already been solved");

        var answer = await _answerRepository.FindAnswerById(request.AnswerId, cancellationToken);

        if (answer is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.ANSWER_ID_NOTFOUND, request.QuestionId));

        if (answer.IsAccepted)
            return GenericResult<GenericResponse>.Failure("Answer is already been accepted");

        question.IsSolved = true;
        answer.IsAccepted = true;

        if (answer.AuthorId != _authenticationContext.UserId)
        {
            var user = await _userRepository.FindUserByIdAsync(answer.AuthorId, cancellationToken);
            user!.Reputation += options.ReputationAcquirePerAction.AnswerAccepted;
        }

        var result = await _questionRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new("Done"))
            : GenericResult<GenericResponse>.Failure(result.Message);

    }
}
