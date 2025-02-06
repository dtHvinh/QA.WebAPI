using WebAPI.Model;
using WebAPI.Utilities;
using WebAPI.Utilities.Params;

namespace WebAPI.Repositories.Base;

public interface IQuestionRepository : IRepository<Question>
{
    /// <summary>
    /// Find a question with almost all references is loaded
    /// </summary>
    Task<Question?> FindQuestionDetailByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Question>> SearchQuestionAsync(QuestionSearchParams searchParams, CancellationToken cancellationToken);
    void MarkAsView(Guid questionId);
    Task SetQuestionTag(Question question, List<Tag> tags);
    Task<Question?> FindQuestionByIdAsync(Guid id, CancellationToken cancellationToken);
    /// <summary>
    /// Update question also update the <see cref="Question.UpdatedAt"/> field.
    /// </summary>
    /// <param name="question"></param>
    void UpdateQuestion(Question question);
    void VoteChange(Question question, Enums.VoteUpdateTypes updateType, int value);
    Task<List<Question>> FindQuestionByUserId(Guid userId, int skip, int take, QuestionSortOrder sortOrder, CancellationToken cancellationToken);
    void SoftDeleteQuestion(Question question);
    Task<int> CountUserQuestion(Guid userId);
    Task<List<Question>> FindQuestionsByTagId(Guid tagId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken);
}
