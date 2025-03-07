using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebAPI.CommandQuery.Commands;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Filters.Requirement;
using WebAPI.Filters.Validation;
using WebAPI.Model;
using WebAPI.Pagination;
using WebAPI.Response;
using WebAPI.Response.AsnwerResponses;
using WebAPI.Response.CommentResponses;
using WebAPI.Response.HistoryResponses;
using WebAPI.Response.QuestionResponses;
using WebAPI.Response.VoteResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Endpoints.Question;

public sealed class QuestionModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(EG.Question)
            .WithTags(nameof(QuestionModule));

        group.MapGet("/user", GetUserQuestionHandler)
            .RequireAuthorization();

        group.MapGet("/", GetQuestionsHandler)
            .RequireAuthorization();

        group.MapGet("/view/{id:int}", ViewQuestionDetailHandler)
            .RequireAuthorization();

        group.MapGet("/search", SearchQuestionHandler)
            .RequireAuthorization();

        group.MapGet("/you_may_like", QuestionYouMayLikeHandler)
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/history", ViewQuestionHistory)
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/similar", GetSimilarQuestion)
            .RequireAuthorization();

        group.MapPost("/", CreateQuestionHandler)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateQuestionDto>>()
            .AddEndpointFilter<ForCreateQuestion>();

        group.MapPost("/{questionId:int}/comment", CommentToQuestionHandler)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommentDto>>()
            .AddEndpointFilter<ForComment>();

        group.MapPost("/{questionId:int}/answer", AnswerToQuestionHandler)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateAnswerDto>>();

        group.MapPost("/{questionId:int}/upvote", UpvoteQuestionHandler)
            .RequireAuthorization()
            .AddEndpointFilter<ForUpvote>();

        group.MapPost("/{questionId:int}/downvote", DownvoteQuestionHandler)
            .RequireAuthorization()
            .AddEndpointFilter<ForDownVote>();

        group.MapDelete("/{id:int}", DeleteQuestionHandler)
            .RequireAuthorization();

        group.MapPut("/", UpdateQuestionHandler)
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateQuestionDto>>();

        group.MapPut("/{questionId:int}/accept/{answerId:int}", AcceptQuestionHandler)
            .RequireAuthorization();

        group.MapPut("/{questionId:int}/close", CloseQuestionHandler)
            .RequireAuthorization();
    }

    private static
        async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> QuestionYouMayLikeHandler(
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetQuestionYouMayLikeQuery(PageArgs.From(pageIndex, pageSize));
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<UpdateQuestionResponse>, ProblemHttpResult>> UpdateQuestionHandler(
            [FromBody] UpdateQuestionDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new UpdateQuestionCommand(dto);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> AcceptQuestionHandler(
            int questionId,
            int answerId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new AcceptAnswerCommand(questionId, answerId);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> CloseQuestionHandler(
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new CloseQuestionCommand(questionId);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<DeleteQuestionResponse>, ProblemHttpResult>> DeleteQuestionHandler(
            int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new DeleteQuestionCommand(id);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<VoteResponse>, ProblemHttpResult>> UpvoteQuestionHandler(
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new CreateQuestionVoteCommand(questionId, true);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<VoteResponse>, ProblemHttpResult>> DownvoteQuestionHandler(
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new CreateQuestionVoteCommand(questionId, false);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<AnswerResponse>, ProblemHttpResult>> AnswerToQuestionHandler(
            [FromBody] CreateAnswerDto dto,
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new CreateAnswerCommand(dto, questionId);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<CommentResponse>, ProblemHttpResult>> CommentToQuestionHandler(
            [FromBody] CreateCommentDto dto,
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new CreateCommentCommand(dto, CommentTypes.Question, questionId);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<Results<Ok<CreateQuestionResponse>, ProblemHttpResult>> CreateQuestionHandler(
        [FromBody] CreateQuestionDto dto,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var cmd = new CreateQuestionCommand(dto);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
        Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> GetUserQuestionHandler(
            int pageIndex,
            int pageSize,
            string orderBy,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetUserQuestionQuery(PageArgs.From(pageIndex, pageSize), orderBy);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
        Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> GetQuestionsHandler(
            int pageIndex,
            int pageSize,
            string orderBy,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetQuestionQuery(orderBy, PageArgs.From(pageIndex, pageSize));

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async
        Task<Results<Ok<GetQuestionResponse>, ProblemHttpResult>> ViewQuestionDetailHandler(
            int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetQuestionDetailQuery(id);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> SearchQuestionHandler(
            [FromQuery] string searchTerm,
            [FromQuery] int tagId,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new SearchQuestionQuery(searchTerm, tagId, PageArgs.From(pageIndex, pageSize));

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<List<QuestionHistoryResponse>>, ProblemHttpResult>> ViewQuestionHistory(
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetQuestionHistoryQuery(questionId);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }


    private static async
        Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> GetSimilarQuestion(
            int questionId,
            int skip,
            int take,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetSimilarQuestionQuery(questionId, skip, take);

        var result = await mediator.Send(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }

        return TypedResults.Ok(result.Value);
    }
}