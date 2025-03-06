using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ICollectionRepository : IRepository<Collection>
{
    void AddToCollection(Question question, Collection questionCollection);
    Task<int> CountByAuthorId(int id, CancellationToken cancellationToken);
    Task<int> CountQuestionInCollection(int id, CancellationToken cancellationToken);
    Task<List<Collection>> FindByAuthorId(int id, CollectionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken);
    Task<Collection?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<Collection>> FindCollections(CollectionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken);
    Task<Collection?> FindDetailById(int id, int skip, int take, CancellationToken cancellationToken);
    Task<List<CollectionRepository.CollectionWithAddStatus>> FindWithAddStatusByAuthorId(int id, int questionId, int skip, int take, CancellationToken cancellationToken = default);
    Task<List<bool>> GetAddStatusAsync(List<int> collectionIds, int questionId, CancellationToken cancellation);
    void RemoveFromCollection(Question question, Collection questionCollection);
    Task<List<Collection>> SearchCollections(string searchTerm, int skip, int take, CancellationToken cancellationToken);
    Task<List<Question>> SearchInCollection(int collectionId, string searchTerm, CancellationToken cancellationToken);
}
