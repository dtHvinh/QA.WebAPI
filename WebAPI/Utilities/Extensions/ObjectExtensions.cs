using WebAPI.Model;

namespace WebAPI.Utilities.Extensions;

public static class ObjectExtensions
{
    public static Question WithCommentCount(this Question question, int count)
    {
        question.CommentCount = count;
        return question;
    }

    public static Question WithAnswerCount(this Question question, int count)
    {
        question.AnswerCount = count;
        return question;
    }
}
