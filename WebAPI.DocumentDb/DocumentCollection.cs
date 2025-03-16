using MongoDB.Driver;
using System.Linq.Expressions;

namespace WebAPI.DocumentDb;

public class DocumentCollection<T>
{
    private readonly IMongoCollection<T> _collection;

    public DocumentCollection(ConnectionSettingsBase connectionSettings, string collectionName)
    {
        var mongoClient = new MongoClient(connectionSettings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(connectionSettings.DatabaseName);

        _collection = mongoDatabase.GetCollection<T>(collectionName);
    }

    public async Task<List<T>> GetAsync()
    {
        var result = _collection.Find(FilterDefinition<T>.Empty);
        return await result.ToListAsync();
    }

    public virtual async Task<List<T>> GetAsync(int skip, int limit, CancellationToken cancellationToken = default)
    {
        var result = _collection.Find(FilterDefinition<T>.Empty).Skip(skip).Limit(limit);
        return await result.ToListAsync(cancellationToken);
    }

    public virtual async Task<List<T>> GetDescAsync(Expression<Func<T, object>> selector, int skip, int limit, CancellationToken cancellationToken = default)
    {
        var result = _collection.Find(FilterDefinition<T>.Empty).SortByDescending(selector).Skip(skip).Limit(limit);
        return await result.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetAsync(FilterDefinition<T> filter)
    {
        var result = _collection.Find(filter);
        return await result.FirstOrDefaultAsync();
    }

    public virtual async Task<T?> GetAsync(FilterDefinition<T> filter, int skip, int limit)
    {
        var result = _collection.Find(filter).Skip(skip).Limit(limit);
        return await result.FirstOrDefaultAsync();
    }

    public virtual async Task InsertAsync(T document, CancellationToken cancellation = default)
    {
        await _collection.InsertOneAsync(document, null, cancellation);
    }

    public virtual async Task InsertManyAsync(IEnumerable<T> documents, CancellationToken cancellation = default)
    {
        await _collection.InsertManyAsync(documents, null, cancellation);
    }

    public virtual async Task<long> DeleteManyAsync()
    {
        var result = await _collection.DeleteManyAsync(FilterDefinition<T>.Empty);
        return result.DeletedCount;
    }

    public virtual async Task<long> CountAsync()
    {
        var result = await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty);
        return result;
    }
}
