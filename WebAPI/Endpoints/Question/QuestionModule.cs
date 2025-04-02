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
            .WithTags(nameof(QuestionModule))
            .WithOpenApi();

        group.MapGet("/user", GetUserQuestionHandler)
            .WithName("GetUserQuestions")
            .WithSummary("Get user's questions")
            .WithDescription("Retrieves a paginated list of questions created by the authenticated user")
            .RequireAuthorization();

        group.MapGet("/", GetQuestionsHandler)
            .WithName("GetQuestions")
            .WithSummary("Get all questions")
            .WithDescription("Retrieves a paginated list of all questions with sorting options")
            .RequireAuthorization();

        group.MapGet("/view/{id:int}", ViewQuestionDetailHandler)
            .WithName("GetQuestionDetail")
            .WithSummary("Get question details")
            .WithDescription("Retrieves detailed information about a specific question including answers and comments")
            .RequireAuthorization();

        group.MapGet("/search", SearchQuestionHandler)
            .WithName("SearchQuestions")
            .WithSummary("Search questions")
            .WithDescription("Search questions by keyword and tag with pagination support")
            .RequireAuthorization();

        group.MapGet("/you_may_like", QuestionYouMayLikeHandler)
            .WithName("GetRecommendedQuestions")
            .WithSummary("Get recommended questions")
            .WithDescription("Retrieves personalized question recommendations for the user")
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/history", ViewQuestionHistory)
            .WithName("GetQuestionHistory")
            .WithSummary("Get question history")
            .WithDescription("Retrieves the edit history of a specific question")
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/comments", GetQuestionComments)
            .WithName("Get Question Comment")
            .WithSummary("Get question comment")
            .WithDescription("Retrieves the comments a specific question")
            .RequireAuthorization();

        group.MapGet("/{questionId:int}/similar", GetSimilarQuestion)
            .WithName("GetSimilarQuestions")
            .WithSummary("Get similar questions")
            .WithDescription("Finds questions that are similar to the specified question")
            .RequireAuthorization();

        group.MapPost("/", CreateQuestionHandler)
            .WithName("CreateQuestion")
            .WithSummary("Create new question")
            .WithDescription("Creates a new question with specified title, content, and tags")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateQuestionDto>>()
            .AddEndpointFilter<ForCreateQuestion>();

        group.MapPost("/{questionId:int}/comment", CommentToQuestionHandler)
            .WithName("AddQuestionComment")
            .WithSummary("Add comment to question")
            .WithDescription("Adds a new comment to the specified question")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateCommentDto>>()
            .AddEndpointFilter<ForComment>();

        group.MapPost("/{questionId:int}/answer", AnswerToQuestionHandler)
            .WithName("AddAnswer")
            .WithSummary("Answer question")
            .WithDescription("Submits an answer to the specified question")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<CreateAnswerDto>>();

        group.MapPost("/{questionId:int}/upvote", UpvoteQuestionHandler)
            .WithName("UpvoteQuestion")
            .WithSummary("Upvote question")
            .WithDescription("Adds an upvote to the specified question")
            .RequireAuthorization()
            .AddEndpointFilter<ForUpvote>();

        group.MapPost("/{questionId:int}/downvote", DownvoteQuestionHandler)
            .WithName("DownvoteQuestion")
            .WithSummary("Downvote question")
            .WithDescription("Adds a downvote to the specified question")
            .RequireAuthorization()
            .AddEndpointFilter<ForDownVote>();

        group.MapDelete("/{id:int}", DeleteQuestionHandler)
            .WithName("DeleteQuestion")
            .WithSummary("Delete question")
            .WithDescription("Removes a question and all its associated content")
            .RequireAuthorization();

        group.MapPut("/duplicate", FlagQuestionDuplicate)
            .RequireAuthorization()
            .WithName("FlagQuestionDuplicate")
            .WithSummary("Flag question as duplicate")
            .WithDescription("Marks a question as a duplicate of another question")
            .AddEndpointFilter<FluentValidation<FlagQuestionDuplicateDto>>();

        group.MapPut("/{questionid}/remove-duplicate-flag", RemoveDuplicateFlag)
            .WithName("RemoveDuplicateFlag")
            .RequireAuthorization();

        group.MapPut("/", UpdateQuestionHandler)
            .WithName("UpdateQuestion")
            .WithSummary("Update question")
            .WithDescription("Updates the content of an existing question")
            .RequireAuthorization()
            .AddEndpointFilter<FluentValidation<UpdateQuestionDto>>();

        group.MapPut("/{questionId:int}/accept/{answerId:int}", AcceptQuestionHandler)
            .WithName("AcceptAnswer")
            .WithSummary("Accept answer")
            .WithDescription("Marks an answer as the accepted solution for the question")
            .RequireAuthorization();

        group.MapPut("/{questionId:int}/close", CloseQuestionHandler)
            .WithName("CloseQuestion")
            .WithSummary("Close question")
            .WithDescription("Marks a question as closed, preventing new answers and comments")
            .RequireAuthorization();

        group.MapPut("/{questionId:int}/re-open", ReopenQuestionHandler)
            .WithName("ReopenQuestion")
            .WithSummary("Re-open a question")
            .WithDescription("Re-open a question")
            .RequireAuthorization();
    }

    private static
        async Task<Results<Ok<List<CommentResponse>>, ProblemHttpResult>> GetQuestionComments(
            int questionId,
            [AsParameters] PageArgs pageArgs,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new GetQuestionCommentQuery(questionId, pageArgs);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<TextResponse>, ProblemHttpResult>> RemoveDuplicateFlag(
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new RemoveDuplicateFlagCommand(questionId);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
    }

    private static
        async Task<Results<Ok<TextResponse>, ProblemHttpResult>> FlagQuestionDuplicate(
            [FromBody] FlagQuestionDuplicateDto dto,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new FlagQuestionDuplicateCommand(dto.QuestionId, dto.DuplicateUrl);
        var result = await mediator.Send(cmd, cancellationToken);
        if (!result.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest(result.Message);
        }
        return TypedResults.Ok(result.Value);
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
        async Task<Results<Ok<TextResponse>, ProblemHttpResult>> AcceptQuestionHandler(
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
        async Task<Results<Ok<TextResponse>, ProblemHttpResult>> CloseQuestionHandler(
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
        async Task<Results<Ok<TextResponse>, ProblemHttpResult>> ReopenQuestionHandler(
            int questionId,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken = default)
    {
        var cmd = new ReopenQuestionCommand(questionId);

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
        var query = new GetQuestionQuery(orderBy, PageArgs.From(pageIndex, pageSize));

        var result = await mediator.Send(query, cancellationToken);

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