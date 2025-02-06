using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagWithQuestionHandler(ITagRepository tagRepository, ICacheService cacheService)
    : IQueryHandler<GetTagWithQuestionQuery, GenericResult<TagWithQuestionResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ICacheService cacheService = cacheService;

    public async Task<GenericResult<TagWithQuestionResponse>> Handle(GetTagWithQuestionQuery request, CancellationToken cancellationToken)
    {
        Tag? tag = await cacheService.GetTagWithQuestion(request.TagId, request.OrderBy,
            request.PageArgs.Page, request.PageArgs.PageSize);

        if (tag == null)
        {
            tag = await _tagRepository.FindTagWithQuestionById(
                request.TagId,
                request.OrderBy switch
                {
                    "Newest" => QuestionSortOrder.Newest,
                    "MostViewed" => QuestionSortOrder.MostViewed,
                    "MostVoted" => QuestionSortOrder.MostVoted,
                    "Solved" => QuestionSortOrder.Solved,
                    _ => QuestionSortOrder.Newest
                },
                (request.PageArgs.Page - 1) * request.PageArgs.PageSize,
                request.PageArgs.PageSize + 1,
                cancellationToken);

            if (tag == null)
                return GenericResult<TagWithQuestionResponse>.Failure(EM.TAG_NOTFOUND);

            await cacheService.SetTagDetail(tag, request.OrderBy, request.PageArgs.Page, request.PageArgs.PageSize);
        }

        var hasNext = tag.Questions.Count == request.PageArgs.PageSize + 1;

        var pagedQuestion = new PagedResponse<GetQuestionResponse>(
            tag.Questions.Select(e => e.ToGetQuestionResponse()).Take(request.PageArgs.PageSize),
            hasNext,
            request.PageArgs.Page,
            request.PageArgs.PageSize)
        {
            TotalCount = tag.QuestionCount,
            TotalPage = NumericCalcHelper.GetTotalPage(tag.QuestionCount, request.PageArgs.PageSize)
        };

        return GenericResult<TagWithQuestionResponse>.Success(tag.ToTagWithQuestionResponse(pagedQuestion));
    }
}
