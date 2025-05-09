﻿using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class SearchQuestionHandler(IQuestionRepository questionRepository, Serilog.ILogger logger)
    : IQueryHandler<SearchQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(
        SearchQuestionQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Args.PageIndex - 1) * request.Args.PageSize;

        var questions =
            request.TagId == 0
                ? await _questionRepository.SearchQuestionWithNoTagAsync(request.Keyword, skip,
                    request.Args.PageSize + 1, cancellationToken)
                : await _questionRepository.SearchQuestionWithTagAsync(
                    request.Keyword, request.TagId, skip, request.Args.PageSize + 1,
                    cancellationToken);

        var hasNext = questions.Documents.Count == request.Args.PageSize + 1;

        var response = new PagedResponse<GetQuestionResponse>(
            questions
                .Documents
                .Take(request.Args.PageSize)
                .Select(e => e.ToGetQuestionResponse()),
            hasNext,
            request.Args.PageIndex,
            request.Args.PageSize)
        {
            TotalCount = (int)questions.Total,
            TotalPage = MathHelper.GetTotalPage((int)questions.Total, request.Args.PageSize)
        };

        _logger.Information("Question search with {SearchKeyword} and tag id {EntityId} returned {Count} results",
            request.Keyword, request.TagId, response.Items.Count());

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(response);
    }
}