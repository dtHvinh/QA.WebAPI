using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Utilities.Response.CommentResponses;

namespace WebAPI.Utilities.Mappers;

public static class CommentMap
{
    public static CommentResponse ToCommentResponse(this Comment comment)
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

    public static Comment ToComment(this CreateCommentDto dto, CommentTypes type, Guid authorId, Guid targetId)
    {
        if (type == CommentTypes.Question)
        {
            return new QuestionComment()
            {
                Content = dto.Content,
                AuthorId = authorId,
                QuestionId = targetId
            };
        }
        else
        {
            return new AnswerComment()
            {
                Content = dto.Content,
                AuthorId = authorId,
                AnswerId = targetId
            };
        }
    }
}
