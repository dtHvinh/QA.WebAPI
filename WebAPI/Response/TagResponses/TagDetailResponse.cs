using WebAPI.Response.QuestionResponses;

namespace WebAPI.Response.TagResponses;

public record TagDetailResponse(
    Guid Id, string Name, string Description, string WikiBody, int QuestionCount, List<GetQuestionResponse> Questions);
