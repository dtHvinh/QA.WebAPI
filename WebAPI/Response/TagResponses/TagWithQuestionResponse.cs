using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;

namespace WebAPI.Response.TagResponses;

public record TagWithQuestionResponse(
    Guid Id, string Name, string Description, string WikiBody, int QuestionCount,
    PagedResponse<GetQuestionResponse> Questions);
