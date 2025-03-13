namespace WebAPI.Response.TagResponses;

public record TagWithWikiBodyResponse(int Id, string Name, string? Description, string? WikiBody, int QuestionCount);
