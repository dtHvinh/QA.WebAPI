using Riok.Mapperly.Abstractions;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.CommentResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper(AllowNullPropertyAssignment = false)]
public static partial class CommentExtensions
{
    public static partial CommentResponse ToCommentResponse(this Comment source);
    public static partial CommentResponse ToCommentResponse(this QuestionComment source);
    public static partial void ApplyUpdate(this UpdateCommentDto source, Comment target);

    private static partial QuestionComment ToQuestionComment(this CreateCommentDto source, int authorId, int questionId);
    private static partial AnswerComment ToAnswerComment(this CreateCommentDto source, int authorId, int answerId);

    public static Comment ToComment(this CreateCommentDto dto, CommentTypes type, int authorId, int targetId)
    {
        if (type == CommentTypes.Question)
        {
            return ToQuestionComment(dto, authorId, targetId);
        }
        else
        {
            return ToAnswerComment(dto, authorId, targetId);
        }
    }
}
