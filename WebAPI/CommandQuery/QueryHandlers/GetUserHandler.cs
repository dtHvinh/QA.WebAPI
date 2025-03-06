using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetUserHandler(
    IUserRepository userRepository,
    IQuestionRepository questionRepository,
    IAnswerRepository answerRepository,
    ICommentRepository commentRepository,
    ICollectionRepository questionCollectionRepository,
    AuthenticationContext authenticationContext,
    IExternalLinkRepository externalLinkRepository)
    : IQueryHandler<GetUserQuery, GenericResult<UserResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly ICollectionRepository _qcRepository = questionCollectionRepository;
    private readonly AuthenticationContext _authContext = authenticationContext;
    private readonly IExternalLinkRepository _externalLinkRepository = externalLinkRepository;

    public async Task<GenericResult<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = request.Username is not null
            ? await _userRepository.FindByUsername(request.Username, cancellationToken)
            : await _userRepository.FindUserByIdAsync(_authContext.UserId, cancellationToken);

        if (user == null)
            return GenericResult<UserResponse>.Failure(string.Format(EM.USERNAME_NOTFOUND, request.Username));

        int questionCount = await _questionRepository.CountUserQuestion(user.Id, cancellationToken);
        int answerCount = await _answerRepository.CountUserAnswer(user.Id, cancellationToken);
        int commentCount = await _commentRepository.CountUserComment(user.Id, cancellationToken);
        int collectionCount = await _qcRepository.CountByAuthorId(user.Id, cancellationToken);
        var userLinks = await _externalLinkRepository.GetUserLinks(user.Id, cancellationToken);

        int totalUpvotes = await _questionRepository.CountUserUpvote(user.Id, cancellationToken);
        int acceptedAnswerCount = await _answerRepository.CountUserAcceptedAnswer(user.Id, cancellationToken);

        var response = user.ToUserResponse();
        response.AnswerCount = answerCount;
        response.QuestionCount = questionCount;
        response.CommentCount = commentCount;
        response.CollectionCount = collectionCount;
        response.TotalUpvotes = totalUpvotes;
        response.AcceptedAnswerCount = acceptedAnswerCount;
        response.ExternalLinks = userLinks.Select(x => x.ToExternalLinkResponse()).ToList();

        if (request.Username == null)
            response.ResourceRight = nameof(ResourceRights.Owner);

        return GenericResult<UserResponse>.Success(response);
    }
}
