using WebAPI.Model;
using WebAPI.Utilities.Response.CommentResponses;

namespace WebAPI.Utilities.Mappers;

public static class CommentMap
{
    public static CommentResponse ToCommentResponse(this QuestionComment comment)
    {
        return new CommentResponse()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Author = comment.Author.ToAuthorResponse()
        };
    }
}
