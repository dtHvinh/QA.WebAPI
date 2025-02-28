using Microsoft.Extensions.Options;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CloseQuestionHandler(
    IQuestionRepository questionRepository,
    IUserRepository userRepository,
    IOptions<ApplicationProperties> applicationProperties,
    QuestionSearchService questionSearchService,
    AuthenticationContext authenticationContext)
    : ICommandHandler<CloseQuestionCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;
    private readonly ApplicationProperties _applicationProperties = applicationProperties.Value;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GenericResponse>> Handle(CloseQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var existQuestion =
            await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (existQuestion is null)
            return GenericResult<GenericResponse>.Failure(string.Format(EM.QUESTION_ID_NOTFOUND, request.QuestionId));

        if (!_authenticationContext.IsModerator())
        {
            return GenericResult<GenericResponse>.Failure(
                string.Format(EM.ROLE_NOT_MEET_REQ, Roles.Moderator));
        }

        existQuestion.IsClosed = true;

        _questionRepository.UpdateQuestion(existQuestion);
        var result = await _questionRepository.SaveChangesAsync(cancellationToken);

        if (result.IsSuccess)
            await _questionSearchService.IndexOrUpdateAsync(existQuestion, cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success("Question closed")
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}