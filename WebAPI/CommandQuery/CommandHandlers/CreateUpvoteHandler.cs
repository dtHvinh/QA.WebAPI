using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateUpvoteHandler(
    IQuestionRepository questionRepository,
    IUpvoteRepository upvoteRepository,
    AuthenticationContext authContext)
    : ICommandHandler<CreateQuestionUpvoteCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository questionRepository = questionRepository;
    private readonly IUpvoteRepository upvoteRepository = upvoteRepository;
    private readonly AuthenticationContext _authContext = authContext;

    public async Task<GenericResult<GenericResponse>> Handle(CreateQuestionUpvoteCommand request, CancellationToken cancellationToken)
    {
        var question = await questionRepository.FindQuestionByIdAsync(request.QuestionId, cancellationToken);

        if (question == null)
        {
            return GenericResult<GenericResponse>.Failure("Question not found");
        }

        var isUpvoteCreated = await upvoteRepository.AddQuestionUpvote(request.QuestionId, _authContext.UserId, cancellationToken);

        if (!isUpvoteCreated)
        {
            return GenericResult<GenericResponse>.Failure("Upvote already exists");
        }

        questionRepository.UpvoteUpdate(question, 1);

        var result = await questionRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<GenericResponse>.Success(new GenericResponse("Upvote created successfully"))
            : GenericResult<GenericResponse>.Failure(result.Message);
    }
}
