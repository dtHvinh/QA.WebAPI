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

    public async Task<List<Collection>> FindCollections(CollectionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken)
    {
        var query = Table.Where(e => e.IsPublic);
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

    public async Task<List<Collection>> SearchCollections(string searchTerm, int skip, int take, CancellationToken cancellationToken)
    {
        var query = Table.Where(e => e.IsPublic && e.Name.Contains(searchTerm));

        return await query.Skip(skip)
                          .Take(take)
                          .Include(e => e.Author)
                          .ToListAsync(cancellationToken);
    }

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

    public async Task<List<CollectionWithAddStatus>> FindWithAddStatusByAuthorId(int id, int questionId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await Table.Where(x => x.AuthorId == id)
                          .Select(x => new CollectionWithAddStatus(x)
                          {
                              IsAdded = x.Questions.Any(q => q.Id == questionId)
                          })
                          .Skip(skip)
                          .Take(take)
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
                          .OrderByDescending(e => e.CreatedAt)
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

    /// <summary>
    /// The IsAdded property is used to determine if the question is already in the collection.
    /// </summary>
    public sealed class CollectionWithAddStatus
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AuthorId { get; set; }
        public AppUser? Author { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = false;
        public int QuestionCount { get; set; }
        public int LikeCount { get; set; }

        public ICollection<Question> Questions { get; set; }

        public CollectionWithAddStatus(Collection collection)
        {
            Id = collection.Id;
            CreatedAt = collection.CreatedAt;
            UpdatedAt = collection.UpdatedAt;
            AuthorId = collection.AuthorId;
            Author = collection.Author;
            Name = collection.Name;
            Description = collection.Description;
            IsPublic = collection.IsPublic;
            QuestionCount = collection.QuestionCount;
            LikeCount = collection.LikeCount;
            Questions = collection.Questions;
        }

        public bool IsAdded { get; set; }
    }
}
