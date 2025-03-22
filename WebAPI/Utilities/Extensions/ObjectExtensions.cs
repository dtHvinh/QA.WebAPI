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

    public static void DoIfTrue(bool expression, Action action)
    {
        if (expression)
        {
            action();
        }
    }

    public static T ReturnIfNotNull<T>(T? value, T @default)
    {
        if (value == null)
        {
            return @default;
        }

        if (typeof(T).IsValueType && value.Equals(default(T)))
        {
            return @default;
        }

        return value;
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
