using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class SearchQuestionInCollectionHandler(ICollectionRepository collectionRepository)
    : IQueryHandler<SearchQuestionInCollectionQuery, GenericResult<List<GetQuestionResponse>>>
{
    private readonly ICollectionRepository _collectionRepository = collectionRepository;

    public async Task<GenericResult<List<GetQuestionResponse>>> Handle(SearchQuestionInCollectionQuery request, CancellationToken cancellationToken)
    {
        var questions = await _collectionRepository.SearchInCollection(request.CollectionId, request.SearchTerm, cancellationToken);

        return GenericResult<List<GetQuestionResponse>>.Success(questions.Select(e => e.ToGetQuestionResponse()).ToList());
    }
}
