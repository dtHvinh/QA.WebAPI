using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpvoteQuestionHandler(IQuestionRepository questionRepository,
                                   IUpvoteRepository upvoteRepository)
    : ICommandHandler<UpvoteQuestionCommand, GenericResult<GenericResponse>>
{
    private readonly IQuestionRepository questionRepository = questionRepository;
    private readonly IUpvoteRepository _upvoteRepository = upvoteRepository;

    public Task<GenericResult<GenericResponse>> Handle(UpvoteQuestionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
