using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CloseQuestionHandler(IQuestionRepository questionRepository)
    : ICommandHandler<CloseQuestionCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<GenericResponse>> Handle(CloseQuestionCommand request, CancellationToken cancellationToken)
    {
        var existQuestion = await _questionRepository.FindQuestionByIdAsync(request.QuestionId, cancellationToken);

        if (existQuestion is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        existQuestion.IsClosed = true;

        _questionRepository.UpdateQuestion(existQuestion);

        var result = await _questionRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Question closed")
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
