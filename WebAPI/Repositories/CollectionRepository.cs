using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICollectionRepository))]
public class CollectionRepository(ApplicationDbContext dbContext) : RepositoryBase<Collection>(dbContext), ICollectionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Collection>> FindByAuthorId(int id, CollectionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken)
    {
        var query = Table.Where(x => x.AuthorId == id);

        query = sortOrder switch
        {
            CollectionSortOrder.MostLiked => query.OrderByDescending(x => x.LikeCount),
            CollectionSortOrder.Newest => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt),
        };

        return await query.Skip(skip)
                          .Take(take)
                          .Include(e => e.Author)
                          .ToListAsync(cancellationToken);
    }

    public async Task<Collection?> FindDetailById(int id, int skip, int take, CancellationToken cancellationToken)
    {
        return await Table.Include(e => e.Author)
                          .Include(e => e.Questions.Skip(skip).Take(take))
                          .ThenInclude(e => e.Author)
                          .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Collection?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> CountQuestionInCollection(int id, CancellationToken cancellationToken)
    {
        return await Table.Where(x => x.Id == id)
                          .Select(x => x.Questions.Count)
                          .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<bool>> GetAddStatusAsync(List<int> collectionIds, int questionId, CancellationToken cancellation)
    {
        return await Table.Where(x => collectionIds.Contains(x.Id))
                          .Select(x => x.Questions.Any(q => q.Id == questionId))
                          .ToListAsync(cancellation);
    }

    public async Task<List<Question>> SearchInCollection(int collectionId, string searchTerm, CancellationToken cancellationToken)
    {
        return await Table.Where(x => x.Id == collectionId)
                          .SelectMany(x => x.Questions)
                          .Include(e => e.Author)
                          .Where(x => x.Title.Contains(searchTerm))
                          .ToListAsync(cancellationToken);
    }

    public void AddToCollection(Question question, Collection questionCollection)
    {
        questionCollection.Questions = [];
        questionCollection.Questions.Add(question);
    }

    public void RemoveFromCollection(Question question, Collection questionCollection)
    {
        _dbContext.Entry(questionCollection).Collection(x => x.Questions).Load();
        questionCollection.Questions.Remove(question);
    }

    public async Task<int> CountByAuthorId(int id, CancellationToken cancellationToken)
    {
        return await Table.CountAsync(x => x.AuthorId == id, cancellationToken);
    }
}
