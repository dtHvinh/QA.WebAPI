using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IQuestionCollectionRepository : IRepository<QuestionCollection>
{
    Task<int> CountByAuthorId(int id, CancellationToken cancellationToken);
    Task<int> CountQuestionInCollection(int id, CancellationToken cancellationToken);
    Task<List<QuestionCollection>> FindByAuthorId(int id, CollectionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken);
    Task<QuestionCollection?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<QuestionCollection?> FindDetailById(int id, int skip, int take, CancellationToken cancellationToken);
    Task<List<bool>> GetAddStatusAsync(List<int> collectionIds, int questionId, CancellationToken cancellation);
}
