using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.CommentResponses;

namespace WebAPI.Utilities.Extensions;

public static class CommentExtensions
{
    public static CommentResponse ToCommentResponse(this Comment comment)
    {
        return new CommentResponse()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreationDate = comment.CreationDate,
            ModificationDate = comment.ModificationDate,
            Author = comment.Author.ToAuthorResponse()
        };
    }

    public static CommentResponse ToCommentResponse(this QuestionComment comment)
    {
        return new CommentResponse()
        {
            Id = comment.Id,
            Content = comment.Content,
            CreationDate = comment.CreationDate,
            ModificationDate = comment.ModificationDate,
            Author = comment.Author.ToAuthorResponse()
        };
    }

    public static Comment UpdateFrom(this Comment comment, UpdateCommentDto dto)
    {
        comment.Content = dto.Content;
        return comment;
    }

    public static Comment ToComment(this CreateCommentDto dto, CommentTypes type, int authorId, int targetId)
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
