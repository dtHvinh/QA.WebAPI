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

        group.MapGet("/", async Task<Results<Ok<PagedResponse<GetQuestionResponse>>, ProblemHttpResult>> (
            int pageIndex,
            int pageSize,
            string order,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new GetUserQuestionQuery(PageArgs.From(pageIndex, pageSize), order);

            var result = await mediator.Send(cmd, cancellationToken);

            if (!result.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest(result.Message);
            }

            return TypedResults.Ok(result.Value);
        })
            .RequireAuthorization();

        group.MapGet("/view/{id:guid}", async Task<Results<Ok<GetQuestionResponse>, ProblemHttpResult>> (
            Guid id,
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
            [FromQuery] string keyword,
            [FromQuery] Guid tagId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default) =>
        {
            var cmd = new SearchQuestionQuery(keyword, tagId, PageArgs.From(page, pageSize));

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

        group.MapPost("/{questionId:guid}/comment",
            async Task<Results<Ok<CommentResponse>, ProblemHttpResult>> (
            [FromBody] CreateCommentDto dto,
            Guid questionId,
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

        group.MapPost("/{questionId:guid}/answer",
            async Task<Results<Ok<AnswerResponse>, ProblemHttpResult>> (
            [FromBody] CreateAnswerDto dto,
            Guid questionId,
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

        group.MapPost("/{questionId:guid}/{action:regex(^(upvote|downvote)$)}",
            async Task<Results<Ok<VoteResponse>, ProblemHttpResult>> (
            Guid questionId,
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

        group.MapDelete("/{id:guid}", async Task<Results<Ok<DeleteQuestionResponse>, ProblemHttpResult>> (
            Guid id,
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

        group.MapPut("/{questionId:guid}/accept/{answerId:guid}",
            async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            Guid questionId,
            Guid answerId,
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

        group.MapPut("/{questionId:guid}/close",
            async Task<Results<Ok<GenericResponse>, ProblemHttpResult>> (
            Guid questionId,
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
