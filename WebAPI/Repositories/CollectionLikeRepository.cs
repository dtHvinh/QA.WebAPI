using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICollectionLikeRepository))]
public class CollectionLikeRepository(ApplicationDbContext dbContext)
    : RepositoryBase<CollectionLike>(dbContext), ICollectionLikeRepository
{
    public async Task<bool> IsLikedByUser(int collectionId, int userId)
    {
        return await Table.AnyAsync(e => e.CollectionId == collectionId && e.AuthorId == userId);
    }

    public async Task UnlikeCollection(Collection collection, int userId,
        CancellationToken cancellationToken = default)
    {
        var toDelete = await Entities.FirstOrDefaultAsync(e => e.CollectionId == collection.Id
                                                               && e.AuthorId == userId, cancellationToken);

        if (toDelete is not null)
            Entities.Remove(toDelete);
    }

    public void LikeCollection(Collection collection, int userId)
    {
        var collectionLike = new CollectionLike
        {
            CollectionId = collection.Id,
            AuthorId = userId
        };

        Entities.Add(collectionLike);
    }
}