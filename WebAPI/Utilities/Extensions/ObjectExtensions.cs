using WebAPI.Model;
using WebAPI.Utilities.Contract;

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

    /// <summary>
    /// Set <see cref="ISoftDeleteEntity.IsDeleted"/> to <see langword="true"/>
    /// </summary>
    /// <param name="softDeleteEntity"></param>
    /// <returns></returns>
    public static ISoftDeleteEntity SolftDelete(this ISoftDeleteEntity softDeleteEntity)
    {
        softDeleteEntity.IsDeleted = true;
        return softDeleteEntity;
    }
}
