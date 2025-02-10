using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.HistoryResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionHistoryHandler(IQuestionHistoryRepository historyRepository)
    : IQueryHandler<GetQuestionHistoryQuery, GenericResult<List<QuestionHistoryResponse>>>
{
    private readonly IQuestionHistoryRepository _historyRepository = historyRepository;

    public async Task<GenericResult<List<QuestionHistoryResponse>>> Handle(GetQuestionHistoryQuery request, CancellationToken cancellationToken)
    {
        var histories = await _historyRepository.FindHistoryWithAuthor(request.QuestionId, cancellationToken);

        return GenericResult<List<QuestionHistoryResponse>>.Success(
            histories.Select(e => e.ToResponse()).ToList());
    }
}
