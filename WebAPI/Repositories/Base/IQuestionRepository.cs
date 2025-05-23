using WebAPI.Model;
using WebAPI.Response;

namespace WebAPI.Repositories.Base;

public interface IQuestionRepository : IRepository<Question>
{
    /// <summary>
    /// Find a question with almost all references is loaded
    /// </summary>
    Task<Question?> FindQuestionDetailByIdAsync(int id, CancellationToken cancellationToken);
    void MarkAsView(int questionId);
    Task SetQuestionTag(Question question, List<Tag> tags);
    Task<Question?> FindQuestionWithAuthorByIdAsync(int id, CancellationToken cancellationToken);
    /// <summary>
    /// Update question also update the <see cref="Question.ModificationDate"/> field.
    /// </summary>
    /// <param name="question"></param>
    Task UpdateQuestion(Question question);
    Task<List<Question>> FindQuestionByUserId(int userId, int skip, int take, QuestionSortOrder sortOrder, CancellationToken cancellationToken);
    void SoftDeleteQuestion(Question question);
    Task<int> CountUserQuestion(int userId, CancellationToken cancellationToken);
    Task<List<Question>> FindQuestionsByTagId(int tagId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken);
    Task<List<Question>> FindQuestion(int skip, int take, QuestionSortOrder sortOrder, CancellationToken cancellationToken);
    Task<Question?> FindQuestionById(int questionId, CancellationToken cancellationToken);
    Task<int> CountUserScore(int userId, CancellationToken cancellationToken);
    Task<Question?> FindQuestionWithTags(int questionId, CancellationToken cancellationToken);
    Task<SearchResult<Question>> SearchQuestionWithNoTagAsync(string keyword, int skip, int take, CancellationToken cancellationToken);
    Task<SearchResult<Question>> SearchQuestionWithTagAsync(string keyword, int tagId, int skip, int take, CancellationToken cancellationToken);
    Task<SearchResult<Question>> SearchSimilarQuestionAsync(int questionId, int skip, int take, CancellationToken cancellationToken);
    Task<SearchResult<Question>> SearchQuestionYouMayLikeAsync(int skip, int take, CancellationToken cancellationToken);
    Task<List<Question>> FindQuestionNoException(int skip, int take, CancellationToken cancellationToken = default);
}
