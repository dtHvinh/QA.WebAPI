﻿using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.QuestionResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetTagQuestionHandler(IQuestionRepository questionRepository) : IQueryHandler<GetTagQuestionQuery, GenericResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<GenericResult<PagedResponse<GetQuestionResponse>>> Handle(GetTagQuestionQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.FindQuestionsByTagId(
                        request.TagId,
                        request.Order switch
                        {
                            "Newest" => QuestionSortOrder.Newest,
                            "MostViewed" => QuestionSortOrder.MostViewed,
                            "MostVoted" => QuestionSortOrder.MostVoted,
                            "Solved" => QuestionSortOrder.Solved,
                            "Draft" => QuestionSortOrder.Draft,
                            _ => QuestionSortOrder.Newest
                        },
                        (request.PageArgs.Page - 1) * request.PageArgs.PageSize,
                        request.PageArgs.PageSize + 1,
                        cancellationToken);

        var hasNext = questions.Count == request.PageArgs.PageSize + 1;

        return GenericResult<PagedResponse<GetQuestionResponse>>.Success(
            new PagedResponse<GetQuestionResponse>(
                questions.Take(request.PageArgs.PageSize).Select(e => e.ToGetQuestionResponse()),
                hasNext,
                request.PageArgs.Page,
                request.PageArgs.PageSize)
        );
    }
}
