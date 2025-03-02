using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ICollectionLikeRepository : IRepository<CollectionLike>
{
    Task<bool> IsLikedByUser(int collectionId, int userId);
    void LikeCollection(Collection collection, int userId);
    Task UnlikeCollection(Collection collection, int userId, CancellationToken cancellationToken = default);
}