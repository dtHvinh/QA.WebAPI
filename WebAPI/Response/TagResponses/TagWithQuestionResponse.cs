using WebAPI.Pagination;
using WebAPI.Response.QuestionResponses;

namespace WebAPI.Response.TagResponses;

public record TagWithQuestionResponse(
    int Id, string Name, string? Description, int QuestionCount,
    PagedResponse<GetQuestionResponse> Questions);
