using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Response.TagResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagWithQuestionHandler(ITagRepository tagRepository)
    : IQueryHandler<GetTagWithQuestionQuery, GenericResult<TagWithQuestionResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;

    public async Task<GenericResult<TagWithQuestionResponse>> Handle(GetTagWithQuestionQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.FindTagWithQuestionById(
                request.TagId,
                Enum.Parse<QuestionSortOrder>(request.OrderBy, true),
                (request.PageArgs.Page - 1) * request.PageArgs.PageSize,
                request.PageArgs.PageSize + 1,
                cancellationToken);

        if (tag == null)
            return GenericResult<TagWithQuestionResponse>.Failure(EM.TAG_NOTFOUND);

        var hasNext = tag.Questions.Count == request.PageArgs.PageSize + 1;

        var pagedQuestion = new PagedResponse<GetQuestionResponse>(
            tag.Questions.Select(e => e.ToGetQuestionResponse()).Take(request.PageArgs.PageSize),
            hasNext,
            request.PageArgs.Page,
            request.PageArgs.PageSize)
        {
            TotalCount = tag.QuestionCount,
            TotalPage = MathHelper.GetTotalPage(tag.QuestionCount, request.PageArgs.PageSize)
        };

        return GenericResult<TagWithQuestionResponse>.Success(tag.ToTagWithQuestionResponse(pagedQuestion));
    }
}
