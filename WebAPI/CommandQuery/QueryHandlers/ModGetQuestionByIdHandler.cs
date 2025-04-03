using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class ModGetQuestionByIdHandler(IQuestionRepository questionRepository)
    : IQueryHandler<ModGetQuestionByIdQuery, GenericResult<GetQuestionResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<GetQuestionResponse>> Handle(ModGetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.FindQuestionWithAuthorByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
        {
            return GenericResult<GetQuestionResponse>.Failure("Question not found");
        }

        return GenericResult<GetQuestionResponse>.Success(question.ToGetQuestionResponse());
    }
}
