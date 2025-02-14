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
        var group = endpoints.MapGroup(EG.Question);

        #region GET

        group.MapGet("/user", async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
            int pageIndex,
            int pageSize,
            string orderBy,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetUserQuestionQuery(PageArgs.From(pageIndex, pageSize), orderBy);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/", async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
            int pageIndex,
            int pageSize,
            string orderBy,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetQuestionQuery(orderBy, PageArgs.From(pageIndex, pageSize));

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/view/{id:int}", async Task<Results<Ok<GetQuestionResponse>, ProblemHttpResult>> (
            int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetQuestionDetailQuery(id);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/search",
            async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
            [FromQuery] string searchTerm,
            [FromQuery] int tagId,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new SearchQuestionQuery(searchTerm, tagId, PageArgs.From(pageIndex, pageSize));

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/history",
            async Task<Results<Ok<List<QuestionHistoryResponse>>, ProblemHttpResult>> (
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetQuestionHistoryQuery(questionId);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/similar",
            async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
            int questionId,
            int skip,
            int take,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
            {
                var cmd = new GetSimilarQuestionQuery(questionId, skip, take);

                var result = await mediator.Send(cmd, cancellationToken);

                if (!result.IsSuccess)
                {
                    return ProblemResultExtensions.BadRequest(result.Message);
                }

                return TypedResults.Ok(result.Value);
            })
            .RequireAuthorization();

        #endregion GET

        #region POST

        group.MapPost("/", async Task<Results<Ok<CreateQuestionResponse>, ProblemHttpResult>> (
            [FromBody] CreateQuestionDto dto,
            [FromServices] IMediator mediator,
            [FromQuery] bool isDraft = false,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateQuestionCommand(dto, isDraft);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateQuestionDto>>();

        group.MapPost("/{questionId:int}/comment",
            async Task<Results<Ok<CommentResponse>, ProblemHttpResult>> (
            [FromBody] CreateCommentDto dto,
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateCommentCommand(dto, CommentTypes.Question, questionId);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommentDto>>()
            .AddEndpointFilter<QuestionCommentReputationRequirement>();

        group.MapPost("/{questionId:int}/answer",
            async Task<Results<Ok<AnswerResponse>, ProblemHttpResult>> (
            [FromBody] CreateAnswerDto dto,
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateAnswerCommand(dto, questionId);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateAnswerDto>>()
            .AddEndpointFilter<AnswerReputationRequirement>();

        group.MapPost("/{questionId:int}/{action:regex(^(upvote|downvote)$)}",
            async Task<Results<Ok<VoteResponse>, ProblemHttpResult>> (
            int questionId,
            string action,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new CreateQuestionVoteCommand(questionId, action == "upvote");

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<UpvoteAndDownvoteReputationRequirement>();

        #endregion POST

        #region DELETE

        group.MapDelete("/{id:int}", async Task<Results<Ok<DeleteQuestionResponse>, ProblemHttpResult>> (
            int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new DeleteQuestionCommand(id);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
             .RequireAuthorization();

        #endregion DELETE

        #region PUT

        group.MapPut("/", async Task<Results<Ok<UpdateQuestionResponse>, ProblemHttpResult>> (
            [FromBody] UpdateQuestionDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new UpdateQuestionCommand(dto);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }
            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateQuestionDto>>();

        group.MapPut("/{questionId:int}/accept/{answerId:int}",
            async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            int questionId,
            int answerId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new AcceptAnswerCommand(questionId, answerId);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }
            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapPut("/{questionId:int}/close",
            async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
            {
                var cmd = new CloseQuestionCommand(questionId);

                var result = await mediator.Send(cmd, cancellationToken);

                if (!result.IsSuccess)
                {
                    return ProblemResultExtensions.BadRequest(result.Message);
                }
                return TypedResults.Ok(result.Value);
            })
            .RequireAuthorization();

        #endregion PUT
    }
}
