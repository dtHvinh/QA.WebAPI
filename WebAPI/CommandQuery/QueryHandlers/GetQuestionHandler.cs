using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Response.QuestionResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetQuestionHandler(IQuestionRepository questionRepository)
    : IQueryHandler<GetQuestionQuery, OperationResult<PagedResponse<GetQuestionResponse>>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<OperationResult<PagedResponse<GetQuestionResponse>>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        //var skip = (request.Args.Page - 1) * request.Args.PageSize;

        //var questions = await _questionRepository.FindQuestionByKeyword(
        //    request.Keyword, skip, request.Args.PageSize + 1, cancellationToken);

        //var hasNext = questions.Count > request.Args.PageSize;

        //questions.RemoveAt(questions.Count - 1);

        //var response = new PagedResponse<GetQuestionResponse>(
        //    questions.Select(e => e.ToGetQuestionResponse()), hasNext, request.Args.Page, request.Args.PageSize);

        //return OperationResult<PagedResponse<GetQuestionResponse>>.Success(response);

        throw new NotImplementedException();
    }
}
