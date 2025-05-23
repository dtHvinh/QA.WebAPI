using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class ModRestoreQuestionHandler(
    IQuestionRepository questionRepository,
    AuthenticationContext authenticationContext)
    : ICommandHandler<ModRestoreQuestionCommand, GenericResult<TextResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(ModRestoreQuestionCommand request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
            return GenericResult<TextResponse>.Failure("You are not authorized to perform this action.");

        var question = await _questionRepository.FindQuestionById(request.QuestionId, cancellationToken);

        if (question == null)
            return GenericResult<TextResponse>.Failure("Question not found");

        question.IsDeleted = false;
        question.ModificationDate = DateTime.UtcNow;

        var res = await _questionRepository.SaveChangesAsync(cancellationToken);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Question restored"))
            : GenericResult<TextResponse>.Failure("Failed to restore question");
    }
}
