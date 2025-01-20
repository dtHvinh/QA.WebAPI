using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateAnswerHandler(IAnswerRepository answerRepository, AuthenticationContext authContext)
    : ICommandHandler<CreateAnswerCommand, GenericResult<AnswerResponse>>
{
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<AnswerResponse>> Handle(
        CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var newAnswer = request.Answer.ToAnswer(_authContext.UserId, request.QuestionId);
        _answerRepository.AddAnswer(newAnswer);

        var result = await _answerRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<AnswerResponse>.Success(newAnswer.ToAnswerResponse())
            : GenericResult<AnswerResponse>.Failure(result.Message);
    }
}
