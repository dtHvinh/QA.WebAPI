using WebAPI.Model;
using WebAPI.Response.HistoryResponses;

namespace WebAPI.Utilities.Extensions;

public static class QuestionHistoryExtensions
{
    public static QuestionHistoryResponse ToResponse(this QuestionHistory obj)
    {
        return new QuestionHistoryResponse
        {
            Id = obj.Id,
            CreationDate = obj.CreationDate,
            ModificationDate = obj.ModificationDate,
            AuthorName = obj.Author?.UserName ?? "Unknown",
            AuthorId = obj.AuthorId,
            QuestionHistoryType = obj.QuestionHistoryType.Name,
            Comment = obj.Comment
        };
    }
}
