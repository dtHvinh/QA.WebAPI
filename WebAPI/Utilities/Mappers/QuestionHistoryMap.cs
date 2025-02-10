using WebAPI.Model;
using WebAPI.Response.HistoryResponses;

namespace WebAPI.Utilities.Mappers;

public static class QuestionHistoryMap
{
    public static QuestionHistoryResponse ToResponse(this QuestionHistory obj)
    {
        return new QuestionHistoryResponse
        {
            Id = obj.Id,
            CreatedAt = obj.CreatedAt,
            UpdatedAt = obj.UpdatedAt,
            AuthorName = obj.Author?.UserName ?? "Unknown",
            AuthorId = obj.AuthorId,
            QuestionHistoryType = obj.QuestionHistoryType,
            Comment = obj.Comment
        };
    }
}
