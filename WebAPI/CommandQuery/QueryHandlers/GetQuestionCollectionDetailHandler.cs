using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CollectionResponses;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCollectionDetailHandler(
    ICollectionRepository collectionRepository,
    ICollectionLikeRepository collectionLikeRepository,
    AuthenticationContext authenticationContext)
    : IQueryHandler<GetCollectionDetailQuery, GenericResult<GetCollectionDetailResponse>>
{
    private readonly ICollectionRepository _collectionRepository = collectionRepository;
    private readonly ICollectionLikeRepository _collectionLikeRepository = collectionLikeRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<GetCollectionDetailResponse>> Handle(GetCollectionDetailQuery request,
        CancellationToken cancellationToken)
    {
        var qc = await _collectionRepository.FindDetailById(
            request.Id,
            request.PageArgs.CalculateSkip(),
            request.PageArgs.PageSize + 1,
            cancellationToken);

        if (qc == null)
        {
            return GenericResult<GetCollectionDetailResponse>.Failure("Question collection not found");
        }

        var hasNext = qc.Questions.Count == request.PageArgs.PageSize + 1;
        var totalCount = await _collectionRepository.CountQuestionInCollection(qc.Id, cancellationToken);

        var questionPaged = new PagedResponse<GetQuestionResponse>(
            qc.Questions.Take(request.PageArgs.PageSize)
                .Select(x => x.ToGetQuestionResponse())
                .ToList(),
            hasNext,
            request.PageArgs.Page,
            request.PageArgs.PageSize)
        {
            TotalCount = totalCount,
            TotalPage = NumericCalcHelper.GetTotalPage(totalCount, request.PageArgs.PageSize)
        };

        var isLikedByUser = await _collectionLikeRepository.IsLikedByUser(qc.Id, _authenticationContext.UserId);

        return GenericResult<GetCollectionDetailResponse>.Success(
            qc.ToCollectionDetailResponse(isLikedByUser, questionPaged)
                .SetResourceRight(_authenticationContext.UserId));
    }
}