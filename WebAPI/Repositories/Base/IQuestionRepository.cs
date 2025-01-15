using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IQuestionRepository : IRepositoryBase<Question>
{
    /// <summary>
    /// Find available question by id
    /// </summary>
    /// <remarks>
    /// Available question is a question that is 
    /// <list type="bullet">
    ///     <item>not draft</item>
    ///     <item>not closed</item>
    ///     <item>not deleted</item>
    /// </list>
    /// </remarks>
    Task<Question?> FindAvailableQuestionByIdAsync(Guid id, CancellationToken cancellationToken);
    void MarkAsView(Guid questionId);
}
